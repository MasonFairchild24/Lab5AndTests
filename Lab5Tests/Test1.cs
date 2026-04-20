using Lab5.Models;
using Lab5.Services;


namespace Lab5Tests

{
    [TestClass]
    public sealed class BookTests
    {
        [TestMethod]
        public void AddBookTest()
        {
            //Arrange (Set up necessary objects required for the test
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            var book = new Book { Title = "Some Random Novel", Author = "Some Author", ISBN = "123" };

            //Act   (Perform the test and store the result)
            service.AddBook(book);

            //Assert (Make sure it happened and got the right results)
            Assert.AreEqual(99, service.GetBooks().Count);
            Assert.AreEqual("Some Random Novel", service.GetBooks()[0].Title);
        }

        [TestMethod]
        public void AssignIdTest()
        {
            //arrange
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);

            //act
            service.AddBook(new Book { Title = "A", Author = "B", ISBN = "C" });

            //Assert
            Assert.AreEqual(1, service.GetBooks()[0].Id);
        }

        [TestMethod]
        public void AddBook_IdsIncrement()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);

            service.AddBook(new Book { Title = "Book 1", Author = "A", ISBN = "1" });
            service.AddBook(new Book { Title = "Book 2", Author = "B", ISBN = "2" });

            Assert.AreEqual(1, service.GetBooks()[0].Id);
            Assert.AreEqual(2, service.GetBooks()[1].Id);
        }

        [TestMethod]
        public void DeleteBook()
        {
            // Arrange
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddBook(new Book { Title = "To Delete", Author = "A", ISBN = "1" });
            var id = service.GetBooks()[0].Id;

            // Act
            service.DeleteBook(id);

            Assert.AreEqual(0, service.GetBooks().Count);
        }

        [TestMethod]
        public void DeleteBook_InvalidId()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddBook(new Book { Title = "Keep Me", Author = "A", ISBN = "1" });

            // Passing an Id that doesn't exist should do nothing
            service.DeleteBook(999);

            Assert.AreEqual(1, service.GetBooks().Count);
        }
    }

    [TestClass]
    public class UserTests
    {

        [TestMethod]
        public void AddUser()
        {
            // Arrange
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            var user = new User { Name = "Alice", Email = "alice@test.com" };

            // Act
            service.AddUser(user);

            Assert.AreEqual(1, service.GetUsers().Count);
            Assert.AreEqual("Alice", service.GetUsers()[0].Name);
        }

        [TestMethod]
        public void AddUser_AssignsId()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);

            service.AddUser(new User { Name = "Bob", Email = "bob@test.com" });

            Assert.AreEqual(1, service.GetUsers()[0].Id);
        }

        [TestMethod]
        public void DeleteUser()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddUser(new User { Name = "Carol", Email = "carol@test.com" });
            var id = service.GetUsers()[0].Id;

            service.DeleteUser(id);

            // List should be empty after deleting the only user
            Assert.AreEqual(0, service.GetUsers().Count);
        }

        [TestMethod]
        public void DeleteUser_InvalidId()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddUser(new User { Name = "Dave", Email = "dave@test.com" });

            service.DeleteUser(999);

            Assert.AreEqual(1, service.GetUsers().Count);
        }
    }

    [TestClass]
    public class BorrowReturnTests
    {

        [TestMethod]
        public void BorrowBook()
        {
            // Arrange
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddBook(new Book { Title = "Borrowed Book", Author = "A", ISBN = "1" });
            service.AddUser(new User { Name = "Eve", Email = "eve@test.com" });
            var bookId = service.GetBooks()[0].Id;
            var userId = service.GetUsers()[0].Id;

            service.BorrowBook(userId, bookId);

            // Book should leave available list and appear in borrowed
            Assert.AreEqual(0, service.GetBooks().Count);
            Assert.AreEqual(1, service.GetBorrowedBooks().Values.First().Count);
        }

        [TestMethod]
        public void BorrowBook_InvalidUser()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddBook(new Book { Title = "Some Book", Author = "A", ISBN = "1" });
            var bookId = service.GetBooks()[0].Id;

            service.BorrowBook(999, bookId);

            Assert.AreEqual(1, service.GetBooks().Count);
            Assert.AreEqual(0, service.GetBorrowedBooks().Count);
        }

        [TestMethod]
        public void BorrowBook_InvalidBook()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddUser(new User { Name = "Frank", Email = "frank@test.com" });
            var userId = service.GetUsers()[0].Id;

            service.BorrowBook(userId, 999);

            Assert.AreEqual(0, service.GetBorrowedBooks().Count);
        }

        [TestMethod]
        public void ReturnBook()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddBook(new Book { Title = "Return Me", Author = "A", ISBN = "1" });
            service.AddUser(new User { Name = "Grace", Email = "grace@test.com" });
            var bookId = service.GetBooks()[0].Id;
            var userId = service.GetUsers()[0].Id;
            service.BorrowBook(userId, bookId);

            // Act
            service.ReturnBook(userId, 0);

            Assert.AreEqual(1, service.GetBooks().Count);
            Assert.AreEqual(0, service.GetBorrowedBooks().Count);
        }

        [TestMethod]
        public void ReturnBook_InvalidIndex()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddBook(new Book { Title = "A Book", Author = "A", ISBN = "1" });
            service.AddUser(new User { Name = "Hank", Email = "hank@test.com" });
            var bookId = service.GetBooks()[0].Id;
            var userId = service.GetUsers()[0].Id;
            service.BorrowBook(userId, bookId);

            // Out of range index should do nothing
            service.ReturnBook(userId, 999);

            Assert.AreEqual(0, service.GetBooks().Count);
            Assert.AreEqual(1, service.GetBorrowedBooks().Values.First().Count);
        }

        [TestMethod]
        public void ReturnBook_RemovesUserWhenNoBooks()
        {
            var books = Path.GetTempFileName();
            var users = Path.GetTempFileName();
            var service = new LibraryService(books, users);
            service.AddBook(new Book { Title = "Last Book", Author = "A", ISBN = "1" });
            service.AddUser(new User { Name = "Ivy", Email = "ivy@test.com" });
            var bookId = service.GetBooks()[0].Id;
            var userId = service.GetUsers()[0].Id;
            service.BorrowBook(userId, bookId);

            service.ReturnBook(userId, 0);

            // User should be removed from dictionary once they have no borrowed books left
            Assert.AreEqual(0, service.GetBorrowedBooks().Count);
        }

    }
}