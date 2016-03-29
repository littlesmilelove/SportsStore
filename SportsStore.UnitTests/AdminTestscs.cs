using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTestscs
    {
        [TestMethod]
        public void IndexContainsAllProucts()
        {
            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product { ProductID = 1, Name = "P1" },
            new Product { ProductID = 2, Name = "P2" },
            new Product { ProductID = 3, Name = "P3" }
            });

            // Arrange - create a controller
            AdminController target = new AdminController(mock.Object);

            // Action
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            // Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void CanEditProduct()
        {
            // Arrange - create the mock repository    
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
            });

            // Arrange - create the controller    
            AdminController target = new AdminController(mock.Object);

            // Act    
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            // Assert    
            Assert.AreEqual(1, p1.ProductID);    
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void CannotEditNonexistentProduct()
        {
            // Arrange - create the mock repository    
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
            });

            // Arrange - create the controller    
            AdminController target = new AdminController(mock.Object);

            // Act    
            Product result = (Product)target.Edit(4).ViewData.Model;

            // Assert    
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CanSaveValidChanges()
        {
            // Arrange - create mock repository    
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            // Arrange - create the controller   
            AdminController target = new AdminController(mock.Object);

            // Arrange - create a product    
            Product product = new Product {Name = "Test"};

            // Act - try to save the product    
            ActionResult result = target.Edit(product);

            // Assert - check that the repository was called    
            mock.Verify(m => m.SaveProduct(product));

            // Assert - check the method result type    
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CannotSaveInvalidChanges()
        {
            // Arrange - create mock repository    
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            // Arrange - create the controller    
            AdminController target = new AdminController(mock.Object);

            // Arrange - create a product    
            Product product = new Product { Name = "Test" };

            // Arrange - add an error to the model state    
            target.ModelState.AddModelError("error", "error");    
            
            // Act - try to save the product
            ActionResult result = target.Edit(product);

            // Assert - check that the repository was not called    
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            // Assert - check the method result type    
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CanDeleteValidProducts()
        {
            // Arrange - create a Product    
            Product prod = new Product { ProductID = 2, Name = "Test" };

            // Arrange - create the mock repository    
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                prod,
                new Product {ProductID = 3, Name = "P3"},
            });

            // Arrange - create the controller    
            AdminController target = new AdminController(mock.Object);

            // Act - delete the product    
            target.Delete(prod.ProductID);

            // Assert - ensure that the repository delete method was    
            // called with the correct Product    
            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }
    }
}
