using Xamarin.Forms;
using UIKit;
using TalentPlus.Shared.Helpers;
using TalentPlus.iOS;
using XLabs.Forms.Controls;
using TalentPlus.Shared;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using CoreAnimation;
using System.Drawing;
using CoreGraphics;

[assembly: ExportRenderer (typeof(RoundedBox), typeof(RoundedBoxRenderer))]
namespace TalentPlus.iOS
{
	public class RoundedBoxRenderer: ViewRenderer
	{
		protected RoundedRectView _roundRectView;

		public RoundedBoxRenderer () : base ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged (e);
		
			if (e.OldElement == null) {
				_roundRectView = InitRoundRectView (Element as RoundedBox);
				SetNativeControl (_roundRectView);
			}
		}

		private RoundedRectView InitRoundRectView(RoundedBox roundedBox)
		{
			UIRectCorner corner = UIRectCorner.AllCorners;

			if (roundedBox.RoundedSide == RoundedBox.RoundedSideType.Top) {
				corner = UIRectCorner.TopLeft | UIRectCorner.TopRight;
			} else if (roundedBox.RoundedSide == RoundedBox.RoundedSideType.Bottom) {
				corner = UIRectCorner.BottomLeft | UIRectCorner.BottomRight;
			} 

			return new RoundedRectView (this.Bounds, 
										roundedBox.BackgroundColor.ToUIColor(), 
										corner, 
										(float)roundedBox.CornerRadius); 
		}

	}

	public class RoundedRectView : UIView
	{
		private const float RADIUS_DEFAULT = 25f;
		public static UIRectCorner RoundedTopCorners = UIRectCorner.TopLeft | UIRectCorner.TopRight;
		public static UIRectCorner RoundedBottomCorners = UIRectCorner.BottomLeft | UIRectCorner.BottomRight;
		public static UIRectCorner RoundedLeftCorners = UIRectCorner.TopLeft | UIRectCorner.BottomLeft;
		public static UIRectCorner RoundedRightCorners = UIRectCorner.TopRight | UIRectCorner.BottomRight;

		private float fCornerRadius;

		public override UIViewAutoresizing AutoresizingMask {
			get {
				return base.AutoresizingMask;
			}
			set {
				base.AutoresizingMask = value;
				this.UpdateMask ();
			}
		}

		public override CGRect Bounds {
			get {
				return base.Bounds;
			}
			set {
				base.Bounds = value;
				this.UpdateMask ();
			}
		}

		public override CGRect Frame {
			get {
				return base.Frame;
			}
			set {
				base.Frame = value;
				this.UpdateMask ();
			}
		}

		public UIRectCorner eRoundedCorners;

		public RoundedRectView () : base ()
		{
			this.fCornerRadius = RADIUS_DEFAULT;
			this.eRoundedCorners = UIRectCorner.AllCorners;
			this.UpdateMask ();
		}

		public RoundedRectView (RectangleF rect) : base (rect)
		{
			this.fCornerRadius = RADIUS_DEFAULT;
			this.eRoundedCorners = UIRectCorner.AllCorners;
			this.UpdateMask ();
		}
			
		public RoundedRectView (RectangleF rect, UIColor oBackgroundColor) : base (rect)
		{
			this.fCornerRadius = RADIUS_DEFAULT;
			this.eRoundedCorners = UIRectCorner.AllCorners;
			this.BackgroundColor = oBackgroundColor;
			this.UpdateMask ();
		}

		public RoundedRectView (CGRect rect, UIColor oBackgroundColor, UIRectCorner eCornerFlags, float radius) : base (rect)
		{
			this.fCornerRadius = radius;
			this.eRoundedCorners = eCornerFlags;
			this.BackgroundColor = oBackgroundColor;
			this.UpdateMask ();
		}
			
		private void UpdateMask ()
		{
			UIBezierPath oMaskPath = UIBezierPath.FromRoundedRect (this.Bounds, this.eRoundedCorners, new SizeF (this.fCornerRadius, this.fCornerRadius));

			CAShapeLayer oMaskLayer = new CAShapeLayer ();
			oMaskLayer.Frame = this.Bounds;
			oMaskLayer.Path = oMaskPath.CGPath;

			this.Layer.Mask = oMaskLayer;
		}
	}

	//	public class RoundedBoxRenderer : BoxRenderer
	//	{
	//		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
	//		{
	//			if (Element != null)
	//			{
	//				Layer.MasksToBounds = true;
	//				UpdateCornerRadius(Element as RoundedBox);
	//			}
	//		}
	//
	//		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
	//		{
	//			base.OnElementPropertyChanged(sender, e);
	//
	//			if (e.PropertyName == RoundedBox.CornerRadiusProperty.PropertyName)
	//			{
	//				UpdateCornerRadius(Element as RoundedBox);
	//			}
	//		}
	//
	//		private void UpdateCornerRadius(RoundedBox box)
	//		{
	//			//Layer.CornerRadius = (float)box.CornerRadius;
	//
	//			if (box.RoundedSide == RoundedBox.RoundedSideType.Top)
	//			{
	//				RoundCornersOnView(this.NativeView, true, true, false, false, (float)box.CornerRadius);
	//			}
	//			else if (box.RoundedSide == RoundedBox.RoundedSideType.Bottom)
	//			{
	//				RoundCornersOnView(this.NativeView,  false, false, true, true, (float)box.CornerRadius);
	//			}
	//			else
	//			{
	//				Layer.CornerRadius = (float)box.CornerRadius;
	//			}
	//		}
	//
	//		private void RoundCornersOnView(UIView view, bool topLeft, bool topRight, bool bottomLeft, bool bottomRight, float radius)
	//		{
	//			if (topLeft || topRight || bottomLeft || bottomRight) {
	//				UIRectCorner corner = 0;
	//
	//				if (topLeft) {
	//					corner = corner | UIRectCorner.TopLeft;
	//				}
	//				if (topRight) {
	//					corner = corner | UIRectCorner.TopRight;
	//				}
	//				if (bottomLeft) {
	//					corner = corner | UIRectCorner.BottomLeft;
	//				}
	//				if (bottomRight) {
	//					corner = corner | UIRectCorner.BottomRight;
	//				}
	//
	//				UIView roundedView = view;
	//
	//				var maskPath = UIBezierPath.FromRoundedRect (roundedView.Bounds, corner, new CoreGraphics.CGSize (radius, radius));
	//
	//				CAShapeLayer maskLayer = new CAShapeLayer ();
	//				maskLayer.Frame = roundedView.Bounds;
	//				maskLayer.Path = maskPath.CGPath;
	//				roundedView.Layer.Mask = maskLayer;
	//			}
	//		}
	//	}
}