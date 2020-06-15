using System.Collections.Generic;
using System.Linq;

sealed class BindingItem
{
    private HashSet<Ball> _items = new HashSet<Ball>();
    public bool IsRoot;
    public List<Ball> Items => _items.ToList();
    
    public BindingItem(Ball ball, bool isRoot)
    {
        IsRoot = isRoot;
        Add(ball);
    }

    public void SetItems(List<Ball> items)
    {
        this._items = new HashSet<Ball>(items);
    }
    public void SetRoot(bool isRoot)
    {
        IsRoot = isRoot;
    }
    public void Add(Ball ball)
    {
        _items.Add(ball);
    }

    public void Remove(Ball ball)
    {
        _items.Remove(ball);
    }

    public void ClearAll()
    {
        _items.Clear();
    }
}

public static class Binding
{
    private static Loggger log = LoggerProvider.get("Binding");
    private static Dictionary<Ball, BindingItem> _binding = new Dictionary<Ball, BindingItem>();

    public static List<Ball> GetBindingItems(Ball ball)
    {
        if (_binding.ContainsKey(ball))
        {
            return _binding[ball].Items;
        }
        return new List<Ball>();
    }
    public static bool BindingItems(Ball ball, ref List<Ball> items)
    {
        if (_binding.ContainsKey(ball))
        {
            items = _binding[ball].Items;
            return true;
        }

        return false;
    }
    public static void AddBindings(Ball ball, List<Ball> balls)
    {
        foreach (Ball bind in balls)
        {
            // check duplicate binding
            foreach (Ball _b in GetBindingItems(bind))
            {
                if (Equals(_b.position, ball.position))
                {
//                    log.i($"not bind equalt {_b.position} == {ball.position}");
                    return;
                }
            }
            AddBing(ball, bind);
        }
        
    }
    public static void AddBing(Ball ball, Ball bind, bool IsRoot = false)
    {
        // if two object is root to binding not possible
        //if (ball.IsBindingRoot && bind.IsBindingRoot) return;
        log.i($"bind {ball} => {bind}");
        if (!_binding.ContainsKey(ball))
            _binding[ball] = new BindingItem(bind, IsRoot);
        else
            _binding[ball].Add(bind);
    }
    public static void RemoveBing(Ball ball, Ball bind)
    {
        if (_binding.ContainsKey(ball))
            _binding[ball].Remove(bind);
    }
    public static void RemoveAllBindingsFromObject(Ball ball)
    {
        if (_binding.ContainsKey(ball))
        {
            List<Ball> items = new List<Ball>();
            if (!BindingItems(ball, ref items)) return;
            foreach (Ball item in items)
            {
                RemoveBing(item, ball);
            }
            
        }
    }
    public static void SetRootProperty(Ball ball, bool isRoot)
    {
        if (_binding.ContainsKey(ball))
            _binding[ball].SetRoot(isRoot);
    }

    public static List<Ball> GetBallsWithId(Ball ball)
    {
        List<Ball> result = new List<Ball>();

        return result;
    }
}

