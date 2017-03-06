using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace WebApi.Tests
{
    public class ShoppingListTests
    {
        [Fact]
        public void Post_AddNewItem_201Created()
        {
            string postPage = TcpHelper.SendRequest(PostItem(0,"firstitem"));
            string firstLine = postPage.Split('\n')[0];
            
            Assert.Contains("201 Created", firstLine, StringComparison.InvariantCultureIgnoreCase);
                      
            TcpHelper.SendRequest(GetItem(0));
            string getPage = TcpHelper.SendRequest(GetItem(0));
            string firstLine1 = getPage.Split('\n')[0];
            string body = getPage.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None)[1];
            ShoppingItem item = JsonConvert.DeserializeObject<ShoppingItem>(body);

            Assert.Contains("200 OK", firstLine1, StringComparison.InvariantCultureIgnoreCase);
            Assert.Equal(0, item.id);
            Assert.Equal("firstitem", item.name);
            
            TcpHelper.SendRequest(DeleteItem(0));

        }

        [Fact]
        public void Post_ItemWithSameId_409Conflict()
        {
            TcpHelper.SendRequest(PostItem(0, "firstitem"));

            string page = TcpHelper.SendRequest(PostItem(0, "firstitem"));
            string firstLine = page.Split('\n')[0];
            Assert.Contains("409 Conflict", firstLine, StringComparison.InvariantCultureIgnoreCase);

            TcpHelper.SendRequest(DeleteItem(0));
        }

        [Fact]
        public void Post_ItemWithSameName_201Created()
        {
            TcpHelper.SendRequest(PostItem(0, "firstitem"));

            string page = TcpHelper.SendRequest(PostItem(1, "firstitem"));
            string firstLine = page.Split('\n')[0];
            Assert.Contains("201 Created", firstLine, StringComparison.InvariantCultureIgnoreCase);

            TcpHelper.SendRequest(DeleteItem(0));
            TcpHelper.SendRequest(DeleteItem(1));

        }

        [Fact]
        public void Post_ItemWithNullName_()
        {
            string page = TcpHelper.SendRequest(PostItem(0, null));
            string firstLine = page.Split('\n')[0];
            Assert.Contains("400 Bad Request", firstLine, StringComparison.InvariantCultureIgnoreCase);
        }


        [Fact]
        public void Delete_AddedItem_200OK()
        {
            TcpHelper.SendRequest(PostItem(0, "Test"));

            string deletePage = TcpHelper.SendRequest(DeleteItem(0));
            string firstLine = deletePage.Split('\n')[0];
            Assert.Contains("200 OK", firstLine, StringComparison.InvariantCultureIgnoreCase);

            string getPage = TcpHelper.SendRequest(GetItem(0));
            string firstLine1 = getPage.Split('\n')[0];
            Assert.Contains("404 Not Found", firstLine1, StringComparison.InvariantCultureIgnoreCase);

        }

        [Fact]
        public void Delete_NonExistingItem_404NotFound()
        {
            TcpHelper.SendRequest(PostItem(0, "Test"));

            string deletePage = TcpHelper.SendRequest(DeleteItem(0));
            string firstLine = deletePage.Split('\n')[0];
            Assert.Contains("200 OK", firstLine, StringComparison.InvariantCultureIgnoreCase);

            deletePage = TcpHelper.SendRequest(DeleteItem(0));
            string firstLine1 = deletePage.Split('\n')[0];
            Assert.Contains("404 Not Found", firstLine1, StringComparison.InvariantCultureIgnoreCase);

        }


        [Fact]
        public void GetNewAddedItem_ItemFound_200OK()
        {
            TcpHelper.SendRequest(PostItem(3, "test"));
            string page = TcpHelper.SendRequest(GetItem(3));
            string firstLine = page.Split('\n')[0];
            string body = page.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None)[1];
            ShoppingItem item = JsonConvert.DeserializeObject<ShoppingItem>(body);

            Assert.Contains("200 OK", firstLine, StringComparison.InvariantCultureIgnoreCase);
            Assert.Equal(3, item.id);
            Assert.Equal("test", item.name);

            TcpHelper.SendRequest(DeleteItem(3));
        }

        [Fact]
        public void GetNonExistingItem_404NotFound()
        {
            TcpHelper.SendRequest(PostItem(3, "test"));
            string getPage = TcpHelper.SendRequest(GetItem(3));
            string firstLine = getPage.Split('\n')[0];
            string body = getPage.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None)[1];
            ShoppingItem item = JsonConvert.DeserializeObject<ShoppingItem>(body);

            Assert.Contains("200 OK", firstLine, StringComparison.InvariantCultureIgnoreCase);
            Assert.Equal(3, item.id);
            Assert.Equal("test", item.name);

            TcpHelper.SendRequest(DeleteItem(3));
            getPage = TcpHelper.SendRequest(GetItem(3));
            firstLine = getPage.Split('\n')[0];
            
            Assert.Contains("404 Not Found", firstLine, StringComparison.InvariantCultureIgnoreCase);

        }



    
        private string PostItem(int id, string name)
        {
            string httpHeader = @"POST /WebApi/api/ShoppingList HTTP/1.1
Host: localhost
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: 9c0b8d46-9e7e-3168-eda6-565232d99f51";
            ShoppingItem item = new ShoppingItem();
            item.id = id;
            item.name = name;

            string httpBody = JsonConvert.SerializeObject(item);
            string ContentLenght = string.Format("Content-Length: {0}", httpBody.Length);
            string httpRequest = string.Format("{0}\n{1}\n\n{2}", httpHeader, ContentLenght, httpBody);

            return httpRequest;
        }

        private string GetItem(int id)
        {
            string firstLine = string.Format("GET /WebApi/api/ShoppingList?id={0} HTTP/1.1\n", id);
            string header = @"Host: localhost
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: c25a698f-f640-263e-d884-236cda3b1fce

";
            string httpRequest = string.Format("{0}{1}", firstLine, header);
            return httpRequest;
        }
    
        private string GetItems()
        {
            string firstLine ="GET /WebApi/api/ShoppingList HTTP/1.1\n";
            string header = @"Host: localhost
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: c25a698f-f640-263e-d884-236cda3b1fce

";
            string httpRequest = string.Format("{0}{1}", firstLine, header);
            return httpRequest;
        }

        private string DeleteItem(int id)
        {
            string firstLine = string.Format("DELETE /WebApi/api/ShoppingList?id={0} HTTP/1.1\n", id);
            string header = @"Host: localhost
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: e10af258-b136-9e1f-cd3f-44c7320d3e85

";
            string httpRequest = string.Format("{0}{1}", firstLine, header);
            return httpRequest;
        }
    
    }
}
