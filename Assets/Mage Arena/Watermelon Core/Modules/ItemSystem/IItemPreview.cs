using UnityEngine;

namespace Watermelon
{
    public interface IItemPreview
    {
        Sprite Preview { get; }

        Color BackgroundColor { get; }
        Sprite BackgroundSprite { get; }
        Sprite FrameSprite { get; }
        
        int Amount { get; }
    }
}
