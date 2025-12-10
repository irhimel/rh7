using SQLite;
using RH7Accounting.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RH7Accounting.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public DatabaseService()
        {
        }

        private async Task Init()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "accounting.db");
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<Customer>();
            await _database.CreateTableAsync<Invoice>();
            await _database.CreateTableAsync<InvoiceItem>();
        }

        // Customer operations
        public async Task<List<Customer>> GetCustomersAsync()
        {
            await Init();
            return await _database.Table<Customer>()
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            await Init();
            return await _database.Table<Customer>()
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveCustomerAsync(Customer customer)
        {
            await Init();
            if (customer.Id == 0)
            {
                customer.CreatedAt = DateTime.UtcNow;
                return await _database.InsertAsync(customer);
            }
            else
            {
                customer.UpdatedAt = DateTime.UtcNow;
                return await _database.UpdateAsync(customer);
            }
        }

        public async Task<int> DeleteCustomerAsync(Customer customer)
        {
            await Init();
            customer.IsDeleted = true;
            customer.UpdatedAt = DateTime.UtcNow;
            return await _database.UpdateAsync(customer);
        }

        // Invoice operations
        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            await Init();
            var invoices = await _database.Table<Invoice>()
                .Where(i => !i.IsDeleted)
                .ToListAsync();

            // Load customer for each invoice
            foreach (var invoice in invoices)
            {
                invoice.Customer = await GetCustomerByIdAsync(invoice.CustomerId);
                invoice.Items = await GetInvoiceItemsAsync(invoice.Id);
            }

            return invoices;
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int id)
        {
            await Init();
            var invoice = await _database.Table<Invoice>()
                .Where(i => i.Id == id && !i.IsDeleted)
                .FirstOrDefaultAsync();

            if (invoice != null)
            {
                invoice.Customer = await GetCustomerByIdAsync(invoice.CustomerId);
                invoice.Items = await GetInvoiceItemsAsync(invoice.Id);
            }

            return invoice;
        }

        public async Task<int> SaveInvoiceAsync(Invoice invoice)
        {
            await Init();
            if (invoice.Id == 0)
            {
                invoice.CreatedAt = DateTime.UtcNow;
                var result = await _database.InsertAsync(invoice);

                // Save invoice items
                foreach (var item in invoice.Items)
                {
                    item.InvoiceId = invoice.Id;
                    await _database.InsertAsync(item);
                }

                return result;
            }
            else
            {
                invoice.UpdatedAt = DateTime.UtcNow;
                return await _database.UpdateAsync(invoice);
            }
        }

        public async Task<int> DeleteInvoiceAsync(Invoice invoice)
        {
            await Init();
            invoice.IsDeleted = true;
            invoice.UpdatedAt = DateTime.UtcNow;
            return await _database.UpdateAsync(invoice);
        }

        // InvoiceItem operations
        private async Task<List<InvoiceItem>> GetInvoiceItemsAsync(int invoiceId)
        {
            await Init();
            return await _database.Table<InvoiceItem>()
                .Where(i => i.InvoiceId == invoiceId)
                .ToListAsync();
        }

        public async Task<int> SaveInvoiceItemAsync(InvoiceItem item)
        {
            await Init();
            if (item.Id == 0)
            {
                item.CreatedAt = DateTime.UtcNow;
                return await _database.InsertAsync(item);
            }
            else
            {
                return await _database.UpdateAsync(item);
            }
        }

        public async Task<int> DeleteInvoiceItemAsync(InvoiceItem item)
        {
            await Init();
            return await _database.DeleteAsync(item);
        }
    }
}
