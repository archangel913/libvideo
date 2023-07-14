using VideoLibrary;

namespace libvideo.test
{
    [TestClass]
    public class DefaultTest
    {
        [TestMethod]
        public void Test1080()
        {
            string query = @"https://www.youtube.com/watch?v=jfobiCq0YUc&ab_channel=EminemMusic";
            string title = @"Eminem - Higher (Official Video) Explicit";
            Exec(query, title);
        }

        [TestMethod]
        public void Test2060()
        {
            string query = @"https://www.youtube.com/watch?v=LXb3EKWsInQ&ab_channel=Jacob%2BKatieSchwarz";
            string title = @"COSTA RICA IN 4K 60fps HDR (ULTRA HD)";
            Exec(query, title);
        }

        [TestMethod]
        public void JsonParseTest()
        {
            string query = @"https://www.youtube.com/watch?v=U2XK_TJZ3PI";
            string title = @"VTORNIK - Money Rain (DEMONBEATS Phonk Remix)";
            Exec(query, title);
        }

        private static void Exec(string query, string title)
        {
            var mem = new MemoryStream();
            var youtube = YouTube.Default;
            var video = youtube.GetVideo(query);
            video.Stream().CopyTo(mem);
            Assert.IsTrue(mem.Length > 0);
            Assert.AreEqual(title, video.Title);
            Console.WriteLine(title + " test done");
            mem.Dispose();
        }
    }
}