using Xunit;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace TestProject1

{
    // Вспомогательный класс для тестирования
    public static class TestProgramBase
    {
        public static void SaveRequests(List<Request> requests, string fileName)
        {
            ArgumentNullException.ThrowIfNull(requests);
            string json = JsonConvert.SerializeObject(requests, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }
        
        public static List<Request> LoadRequests(string fileName)
        {
            if (!File.Exists(fileName))
                return new List<Request>();
                
            string json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<List<Request>>(json) ?? new List<Request>();
        }
    }

    public class Request
    {
        public int Id { get; internal set; }
        public string Description { get; internal set; }
        public string Status { get; internal set; }
    }

    public class ProgramBaseTests
    {
        private readonly string _testFile = "test_requests.json";
        [Fact]
        public void SaveAndLoad_EmptyList()
        {
            // Arrange
            var emptyList = new List<Request>();

            // Act
            TestProgramBase.SaveRequests(emptyList, _testFile);
            var loaded = TestProgramBase.LoadRequests(_testFile);

            // Assert
            Assert.Empty(loaded);
            Assert.True(File.Exists(_testFile));

            // Cleanup
            File.Delete(_testFile);
        }

        [Fact]
        public void SaveAndLoad_SingleRequest()
        {
            // Arrange
            var request = new Request
            {
                Id = 1,
                Description = "Тест",
                Status = "Новая"
            };
            var list = new List<Request> { request };

            // Act
            TestProgramBase.SaveRequests(list, _testFile);
            var loaded = TestProgramBase.LoadRequests(_testFile);

            // Assert
            Assert.Single(loaded);
            Assert.Equal(1, loaded[0].Id);
            Assert.Equal("Тест", loaded[0].Description);
            Assert.Equal("Новая", loaded[0].Status);

            // Cleanup
            File.Delete(_testFile);
        }
    }

}
    
