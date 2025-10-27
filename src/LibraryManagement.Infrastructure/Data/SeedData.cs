using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            try
            {
                // 1. Primeiro, garantir que as tabelas existem
                await context.Database.EnsureCreatedAsync();
                
                // 2. Pequena pausa para garantir que as tabelas foram criadas
                await Task.Delay(1000);

                // 3. Agora tentar inserir dados
                await SeedBooks(context);
                await SeedMembers(context);
                await SeedLoans(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante seed: {ex.Message}");
                // Não relançar a exceção - deixar a aplicação continuar
            }
        }

        private static async Task SeedBooks(ApplicationDbContext context)
        {
            // Verificar se já existem livros
            if (await CanAccessTable(context.Books))
            {
                if (!await context.Books.AnyAsync())
                {
                    var books = new[]
                    {
                        new Book("Domain-Driven Design", "Eric Evans", "9780321125217", 2003, 5),
                        new Book("Clean Code", "Robert C. Martin", "9780132350884", 2008, 3),
                        new Book("The Clean Coder", "Robert C. Martin", "9780137081073", 2011, 2),
                        new Book("Refactoring", "Martin Fowler", "9780201485677", 1999, 4),
                        new Book("Patterns of Enterprise Application Architecture", "Martin Fowler", "9780321127426", 2002, 3),
                        new Book("Clean Architecture", "Robert C. Martin", "9780134494166", 2017, 4),
                        new Book("Implementing Domain-Driven Design", "Vaughn Vernon", "9780321834577", 2013, 3)
                    };

                    await context.Books.AddRangeAsync(books);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Livros inseridos com sucesso!");
                }
            }
        }

        private static async Task SeedMembers(ApplicationDbContext context)
        {
            if (await CanAccessTable(context.Members))
            {
                if (!await context.Members.AnyAsync())
                {
                    var members = new[]
                    {
                        new Member("João", "Silva", "joao.silva@email.com", "+55 11 99999-1111"),
                        new Member("Maria", "Santos", "maria.santos@email.com", "+55 11 99999-2222"),
                        new Member("Pedro", "Oliveira", "pedro.oliveira@email.com", "+55 11 99999-3333"),
                        new Member("Ana", "Costa", "ana.costa@email.com", "+55 11 99999-4444")
                    };

                    await context.Members.AddRangeAsync(members);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Membros inseridos com sucesso!");
                }
            }
        }

        private static async Task SeedLoans(ApplicationDbContext context)
        {
            if (await CanAccessTable(context.Loans) && 
                await CanAccessTable(context.Books) && 
                await CanAccessTable(context.Members))
            {
                if (!await context.Loans.AnyAsync() && 
                    await context.Books.AnyAsync() && 
                    await context.Members.AnyAsync())
                {
                    var book1 = await context.Books.FirstAsync();
                    var member1 = await context.Members.FirstAsync();
                    
                    var loan = new Loan(book1.Id, member1.Id, DateTime.UtcNow.AddDays(-5), 14);
                    await context.Loans.AddAsync(loan);
                    
                    book1.BorrowCopy();
                    context.Books.Update(book1);
                    
                    await context.SaveChangesAsync();
                    Console.WriteLine("Empréstimo inserido com sucesso!");
                }
            }
        }

        private static async Task<bool> CanAccessTable<T>(IQueryable<T> table) where T : class
        {
            try
            {
                // Tentar uma operação simples para verificar se a tabela existe
                await table.AnyAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}