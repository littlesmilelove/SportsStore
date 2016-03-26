using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using Moq;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    /// <summary>
    /// Summary description for CartTests
    /// </summary>
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void CanAddNewLines()
        {
            // Arrange - create some test products            
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // Arrange - create a new cart            
            Cart target = new Cart();

            // Action
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            // Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void CanAddQuantityForExistingLines()
        {
            // Arrange - create some test products    
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // Arrange - create a new cart    
            Cart target = new Cart();

            // Act    
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            // Assert    
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void CanRemoveLine()
        {
            // Arrange - create some test products    
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            // Arrange - create a new cart    
            Cart target = new Cart();

            // Arrange - add some products to the cart    
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            // Act    
            target.RemoveLine(p2);

            // Assert    
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void CalculateCartTotal()
        {
            // Arrange - create some test products    
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // Arrange - create a new cart    
            Cart target = new Cart();

            // Act    
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            // Assert    
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void CanClearContents()
        {
            // Arrange - create some test products    
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // Arrange - create a new cart    
            Cart target = new Cart();

            // Arrange - add some items    
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            // Act - reset the cart    
            target.Clear();

            // Assert    
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void CanAddToCart()
        {
            // Arrange - create the mock repository            
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
            }.AsQueryable());

            // Arrange - create a Cart 
            Cart cart = new Cart();

            // Arrange - create the controller
            CartController target = new CartController(mock.Object);

            // Act - add a product to the cart            
            target.AddToCart(cart, 1, null);

            // Assert            
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void AddingProductToCartGoesToCartScreen()
        {
            // Arrange - create the mock repository            
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
            }.AsQueryable());

            // Arrange - create a Cart            
            Cart cart = new Cart();

            // Arrange - create the controller            
            CartController target = new CartController(mock.Object);

            // Act - add a product to the cart            
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            // Assert            
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void CanViewCartContent()
        {
            // Arrange - create a Cart            
            Cart cart = new Cart();

            // Arrange - create the controller            
            CartController target = new CartController(null);

            // Act - call the Index action method            
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            // Assert            
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }
    }
}
