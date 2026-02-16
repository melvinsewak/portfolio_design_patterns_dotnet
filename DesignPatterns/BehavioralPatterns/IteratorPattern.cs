namespace DesignPatterns.BehavioralPatterns;

/// <summary>
/// Iterator Pattern - Provides a way to access elements of a collection sequentially
/// without exposing its underlying representation
/// </summary>

#region Example 1: Social Media Feed Iterator

public class Post
{
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int Likes { get; set; }
}

public interface IIterator<T>
{
    bool HasNext();
    T Next();
    void Reset();
}

public interface IAggregate<T>
{
    IIterator<T> CreateIterator();
}

public class SocialFeed : IAggregate<Post>
{
    private readonly List<Post> _posts = new();

    public void AddPost(Post post)
    {
        _posts.Add(post);
    }

    public IIterator<Post> CreateIterator()
    {
        return new ChronologicalIterator(_posts);
    }

    public IIterator<Post> CreateReverseIterator()
    {
        return new ReverseChronologicalIterator(_posts);
    }

    public IIterator<Post> CreatePopularIterator()
    {
        return new PopularityIterator(_posts);
    }

    // Chronological iterator (oldest to newest)
    private class ChronologicalIterator : IIterator<Post>
    {
        private readonly List<Post> _posts;
        private int _position = 0;

        public ChronologicalIterator(List<Post> posts)
        {
            _posts = posts.OrderBy(p => p.Timestamp).ToList();
        }

        public bool HasNext() => _position < _posts.Count;

        public Post Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more posts");
            return _posts[_position++];
        }

        public void Reset() => _position = 0;
    }

    // Reverse chronological iterator (newest to oldest)
    private class ReverseChronologicalIterator : IIterator<Post>
    {
        private readonly List<Post> _posts;
        private int _position = 0;

        public ReverseChronologicalIterator(List<Post> posts)
        {
            _posts = posts.OrderByDescending(p => p.Timestamp).ToList();
        }

        public bool HasNext() => _position < _posts.Count;

        public Post Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more posts");
            return _posts[_position++];
        }

        public void Reset() => _position = 0;
    }

    // Popularity iterator (most liked first)
    private class PopularityIterator : IIterator<Post>
    {
        private readonly List<Post> _posts;
        private int _position = 0;

        public PopularityIterator(List<Post> posts)
        {
            _posts = posts.OrderByDescending(p => p.Likes).ToList();
        }

        public bool HasNext() => _position < _posts.Count;

        public Post Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more posts");
            return _posts[_position++];
        }

        public void Reset() => _position = 0;
    }
}

#endregion

#region Example 2: Playlist Iterator with Shuffle

public class Song
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
}

public class Playlist
{
    private readonly List<Song> _songs = new();
    public string Name { get; set; } = string.Empty;

    public void AddSong(Song song)
    {
        _songs.Add(song);
    }

    public IIterator<Song> GetSequentialIterator()
    {
        return new SequentialIterator(_songs);
    }

    public IIterator<Song> GetShuffleIterator()
    {
        return new ShuffleIterator(_songs);
    }

    public IIterator<Song> GetRepeatIterator()
    {
        return new RepeatIterator(_songs);
    }

    private class SequentialIterator : IIterator<Song>
    {
        private readonly List<Song> _songs;
        private int _position = 0;

        public SequentialIterator(List<Song> songs)
        {
            _songs = new List<Song>(songs);
        }

        public bool HasNext() => _position < _songs.Count;

        public Song Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("End of playlist");
            return _songs[_position++];
        }

        public void Reset() => _position = 0;
    }

    private class ShuffleIterator : IIterator<Song>
    {
        private readonly List<Song> _shuffledSongs;
        private int _position = 0;

        public ShuffleIterator(List<Song> songs)
        {
            _shuffledSongs = new List<Song>(songs);
            // Fisher-Yates shuffle
            var random = new Random();
            for (int i = _shuffledSongs.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (_shuffledSongs[i], _shuffledSongs[j]) = (_shuffledSongs[j], _shuffledSongs[i]);
            }
        }

        public bool HasNext() => _position < _shuffledSongs.Count;

        public Song Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("End of playlist");
            return _shuffledSongs[_position++];
        }

        public void Reset() => _position = 0;
    }

    private class RepeatIterator : IIterator<Song>
    {
        private readonly List<Song> _songs;
        private int _position = 0;

        public RepeatIterator(List<Song> songs)
        {
            _songs = new List<Song>(songs);
        }

        public bool HasNext() => _songs.Count > 0; // Always has next in repeat mode

        public Song Next()
        {
            if (_songs.Count == 0)
                throw new InvalidOperationException("Empty playlist");
            
            var song = _songs[_position];
            _position = (_position + 1) % _songs.Count; // Loop back to start
            return song;
        }

        public void Reset() => _position = 0;
    }
}

#endregion

#region Example 3: Binary Tree Iterator

public class TreeNode<T>
{
    public T Value { get; set; }
    public TreeNode<T>? Left { get; set; }
    public TreeNode<T>? Right { get; set; }

    public TreeNode(T value)
    {
        Value = value;
    }
}

public class BinaryTree<T>
{
    public TreeNode<T>? Root { get; set; }

    public IIterator<T> GetInOrderIterator()
    {
        return new InOrderIterator(Root);
    }

    public IIterator<T> GetPreOrderIterator()
    {
        return new PreOrderIterator(Root);
    }

    public IIterator<T> GetPostOrderIterator()
    {
        return new PostOrderIterator(Root);
    }

    public IIterator<T> GetLevelOrderIterator()
    {
        return new LevelOrderIterator(Root);
    }

    // In-order traversal (Left, Root, Right)
    private class InOrderIterator : IIterator<T>
    {
        private readonly Stack<TreeNode<T>> _stack = new();
        private TreeNode<T>? _current;

        public InOrderIterator(TreeNode<T>? root)
        {
            _current = root;
            PushLeft(_current);
        }

        private void PushLeft(TreeNode<T>? node)
        {
            while (node != null)
            {
                _stack.Push(node);
                node = node.Left;
            }
        }

        public bool HasNext() => _stack.Count > 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more elements");

            var node = _stack.Pop();
            PushLeft(node.Right);
            return node.Value;
        }

        public void Reset()
        {
            _stack.Clear();
            PushLeft(_current);
        }
    }

    // Pre-order traversal (Root, Left, Right)
    private class PreOrderIterator : IIterator<T>
    {
        private readonly Stack<TreeNode<T>> _stack = new();

        public PreOrderIterator(TreeNode<T>? root)
        {
            if (root != null)
                _stack.Push(root);
        }

        public bool HasNext() => _stack.Count > 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more elements");

            var node = _stack.Pop();
            
            if (node.Right != null)
                _stack.Push(node.Right);
            if (node.Left != null)
                _stack.Push(node.Left);

            return node.Value;
        }

        public void Reset()
        {
            // Simplified reset - in real scenario, would need to store original root
            throw new NotImplementedException("Reset not implemented for PreOrder");
        }
    }

    // Post-order traversal (Left, Right, Root)
    private class PostOrderIterator : IIterator<T>
    {
        private readonly List<T> _values = new();
        private int _position = 0;

        public PostOrderIterator(TreeNode<T>? root)
        {
            PostOrderTraversal(root);
        }

        private void PostOrderTraversal(TreeNode<T>? node)
        {
            if (node == null) return;
            PostOrderTraversal(node.Left);
            PostOrderTraversal(node.Right);
            _values.Add(node.Value);
        }

        public bool HasNext() => _position < _values.Count;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more elements");
            return _values[_position++];
        }

        public void Reset() => _position = 0;
    }

    // Level-order traversal (Breadth-first)
    private class LevelOrderIterator : IIterator<T>
    {
        private readonly Queue<TreeNode<T>> _queue = new();

        public LevelOrderIterator(TreeNode<T>? root)
        {
            if (root != null)
                _queue.Enqueue(root);
        }

        public bool HasNext() => _queue.Count > 0;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more elements");

            var node = _queue.Dequeue();
            
            if (node.Left != null)
                _queue.Enqueue(node.Left);
            if (node.Right != null)
                _queue.Enqueue(node.Right);

            return node.Value;
        }

        public void Reset()
        {
            throw new NotImplementedException("Reset not implemented for LevelOrder");
        }
    }
}

#endregion

public static class IteratorDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Iterator Pattern Demo ===\n");

        // Example 1: Social Media Feed
        Console.WriteLine("--- Example 1: Social Media Feed ---");
        var feed = new SocialFeed();
        feed.AddPost(new Post { Author = "Alice", Content = "Hello World!", Timestamp = DateTime.Now.AddHours(-5), Likes = 10 });
        feed.AddPost(new Post { Author = "Bob", Content = "Great day!", Timestamp = DateTime.Now.AddHours(-2), Likes = 25 });
        feed.AddPost(new Post { Author = "Charlie", Content = "Check this out!", Timestamp = DateTime.Now.AddHours(-1), Likes = 5 });
        feed.AddPost(new Post { Author = "Diana", Content = "Amazing!", Timestamp = DateTime.Now.AddHours(-3), Likes = 50 });

        Console.WriteLine("\nReverse Chronological (Newest First):");
        var reverseIterator = feed.CreateReverseIterator();
        while (reverseIterator.HasNext())
        {
            var post = reverseIterator.Next();
            Console.WriteLine($"  [{post.Author}] {post.Content} - {post.Likes} likes");
        }

        Console.WriteLine("\nBy Popularity:");
        var popularIterator = feed.CreatePopularIterator();
        while (popularIterator.HasNext())
        {
            var post = popularIterator.Next();
            Console.WriteLine($"  [{post.Author}] {post.Content} - {post.Likes} likes");
        }

        // Example 2: Music Playlist
        Console.WriteLine("\n\n--- Example 2: Music Playlist ---");
        var playlist = new Playlist { Name = "My Favorites" };
        playlist.AddSong(new Song { Title = "Song A", Artist = "Artist 1", Duration = TimeSpan.FromMinutes(3.5) });
        playlist.AddSong(new Song { Title = "Song B", Artist = "Artist 2", Duration = TimeSpan.FromMinutes(4.2) });
        playlist.AddSong(new Song { Title = "Song C", Artist = "Artist 3", Duration = TimeSpan.FromMinutes(3.8) });

        Console.WriteLine("\nSequential playback:");
        var sequential = playlist.GetSequentialIterator();
        while (sequential.HasNext())
        {
            var song = sequential.Next();
            Console.WriteLine($"  ♪ {song.Title} by {song.Artist}");
        }

        Console.WriteLine("\nShuffle mode:");
        var shuffle = playlist.GetShuffleIterator();
        while (shuffle.HasNext())
        {
            var song = shuffle.Next();
            Console.WriteLine($"  ♪ {song.Title} by {song.Artist}");
        }

        Console.WriteLine("\nRepeat mode (first 5 plays):");
        var repeat = playlist.GetRepeatIterator();
        for (int i = 0; i < 5; i++)
        {
            var song = repeat.Next();
            Console.WriteLine($"  ♪ {song.Title} by {song.Artist}");
        }

        // Example 3: Binary Tree Traversal
        Console.WriteLine("\n\n--- Example 3: Binary Tree Traversal ---");
        var tree = new BinaryTree<int>
        {
            Root = new TreeNode<int>(1)
            {
                Left = new TreeNode<int>(2)
                {
                    Left = new TreeNode<int>(4),
                    Right = new TreeNode<int>(5)
                },
                Right = new TreeNode<int>(3)
                {
                    Left = new TreeNode<int>(6),
                    Right = new TreeNode<int>(7)
                }
            }
        };

        Console.WriteLine("\nIn-Order (Left, Root, Right):");
        var inOrder = tree.GetInOrderIterator();
        Console.Write("  ");
        while (inOrder.HasNext())
            Console.Write($"{inOrder.Next()} ");
        Console.WriteLine();

        Console.WriteLine("\nPre-Order (Root, Left, Right):");
        var preOrder = tree.GetPreOrderIterator();
        Console.Write("  ");
        while (preOrder.HasNext())
            Console.Write($"{preOrder.Next()} ");
        Console.WriteLine();

        Console.WriteLine("\nPost-Order (Left, Right, Root):");
        var postOrder = tree.GetPostOrderIterator();
        Console.Write("  ");
        while (postOrder.HasNext())
            Console.Write($"{postOrder.Next()} ");
        Console.WriteLine();

        Console.WriteLine("\nLevel-Order (Breadth-First):");
        var levelOrder = tree.GetLevelOrderIterator();
        Console.Write("  ");
        while (levelOrder.HasNext())
            Console.Write($"{levelOrder.Next()} ");
        Console.WriteLine();
    }
}
