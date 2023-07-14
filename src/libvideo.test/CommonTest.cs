using VideoLibrary;

namespace libvideo.test
{
    [TestClass]
    public class CommonTest
    {
        [TestMethod]
        public void CommonTest1()
        {
            string query = @"https://www.youtube.com/watch?v=ZRtdQ81jPUQ";
            string title = @"YOASOBI「アイドル」 Official Music Video";
            Exec(query, title);
        }

        [TestMethod]
        public void CommonTest2()
        {
            string query = @"https://www.youtube.com/watch?v=fJ9rUzIMcZQ";
            string title = @"Queen – Bohemian Rhapsody (Official Video Remastered)";
            Exec(query, title);
        }

        [TestMethod]
        public void CommonTest3()
        {
            string query = @"https://www.youtube.com/watch?v=dHXC_ahjtEE";
            string title = @"Chinozo  'グッバイ宣言' feat.FloweR";
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