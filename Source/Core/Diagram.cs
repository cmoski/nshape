/******************************************************************************
  Copyright 2009-2011 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using Dataweb.NShape.Advanced;


namespace Dataweb.NShape {

	/// <summary>
	/// Describes export image file format.
	/// </summary>
	public enum ImageFileFormat {
		/// <summary>Specifies bitmap (BMP) image format.</summary>
		Bmp,
		/// <summary>Specifies the enhanced Windows metafile image format (EMF).</summary>
		Emf,
		/// <summary>Specifies the Graphics Interchange Format (GIF) image format.</summary>
		Gif,
		/// <summary>Specifies the Joint Photographic Experts Group (JPEG) image format.</summary>
		Jpeg,
		/// <summary>Specifies the W3C Portable Network Graphics (PNG) image format.</summary>
		Png,
		/// <summary>Specifies the Tag Image File Format (TIFF) image format.</summary>
		Tiff,
		/// <summary>Specifies the enhanced Windows metafile plus image format (EMF).</summary>
		EmfPlus,
		/// <summary>Specifies the Scalable Vector Graphics file format (SVG).</summary>
		Svg
	}


	#region Layers

	/// <summary>
	/// Describes the layers a shape is part of.
	/// </summary>
	[Flags]
	public enum LayerIds {
		/// <summary>No Layers.</summary>
		None = 0x0,
		/// <summary>Layer 1</summary>
		Layer01 = 0x00000001,
		/// <summary>Layer 2</summary>
		Layer02 = 0x00000002,
		/// <summary>Layer 3</summary>
		Layer03 = 0x00000004,
		/// <summary>Layer 4</summary>
		Layer04 = 0x00000008,
		/// <summary>Layer 5</summary>
		Layer05 = 0x00000010,
		/// <summary>Layer 6</summary>
		Layer06 = 0x00000020,
		/// <summary>Layer 7</summary>
		Layer07 = 0x00000040,
		/// <summary>Layer 8</summary>
		Layer08 = 0x00000080,
		/// <summary>Layer 9</summary>
		Layer09 = 0x00000100,
		/// <summary>Layer 10</summary>
		Layer10 = 0x00000200,
		/// <summary>Layer 11</summary>
		Layer11 = 0x00000400,
		/// <summary>Layer 12</summary>
		Layer12 = 0x00000800,
		/// <summary>Layer 13</summary>
		Layer13 = 0x00001000,
		/// <summary>Layer 14</summary>
		Layer14 = 0x00002000,
		/// <summary>Layer 15</summary>
		Layer15 = 0x00004000,
		/// <summary>Layer 16</summary>
		Layer16 = 0x00008000,
		/// <summary>Layer 17</summary>
		Layer17 = 0x00010000,
		/// <summary>Layer 18</summary>
		Layer18 = 0x00020000,
		/// <summary>Layer 19</summary>
		Layer19 = 0x00040000,
		/// <summary>Layer 20</summary>
		Layer20 = 0x00080000,
		/// <summary>Layer 21</summary>
		Layer21 = 0x00100000,
		/// <summary>Layer 22</summary>
		Layer22 = 0x00200000,
		/// <summary>Layer 23</summary>
		Layer23 = 0x00400000,
		/// <summary>Layer 24</summary>
		Layer24 = 0x00800000,
		/// <summary>Layer 25</summary>
		Layer25 = 0x01000000,
		/// <summary>Layer 26</summary>
		Layer26 = 0x02000000,
		/// <summary>Layer 27</summary>
		Layer27 = 0x04000000,
		/// <summary>Layer 28</summary>
		Layer28 = 0x08000000,
		/// <summary>Layer 29</summary>
		Layer29 = 0x10000000,
		/// <summary>Layer 30</summary>
		Layer30 = 0x20000000,
		/// <summary>Layer 31</summary>
		Layer31 = 0x40000000,
		/// <summary>All available layers.</summary>
		All = int.MaxValue	// 0xFFFFFFFF results in a type mismatch compiler error
	}


	/// <summary>
	/// A layer used for grouoping shapes on a diagram.
	/// </summary>
	/// <status>reviewed</status>
	[TypeDescriptionProvider(typeof(TypeDescriptionProviderDg))]
	public class Layer {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		/// <param name="name"></param>
		public Layer(string name) {
			this.name = name;
		}


		/// <summary>
		/// The <see cref="T:Dataweb.NShape.LayerIds" /> value used to identify the <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		public LayerIds Id {
			get { return id; }
			internal set { id = value; }
		}


		/// <summary>
		/// The language independent name of the <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		[RequiredPermission(Permission.ModifyData)]
		public string Name {
			get { return name; }
			internal set { name = value; }
		}


		/// <summary>
		/// The localized title of the <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		[RequiredPermission(Permission.Present)]
		public string Title {
			get { return string.IsNullOrEmpty(title) ? name : title; }
			set {
				if (value == name || string.IsNullOrEmpty(value))
					title = null;
				else title = value;
			}
		}


		/// <summary>
		/// Specifies the minimum zoom level where the <see cref="T:Dataweb.NShape.Layer" /> is still visible. On lower zoom levels, the layer will be hidden automatically.
		/// </summary>
		[RequiredPermission(Permission.Present)]
		public int LowerZoomThreshold {
			get { return lowerZoomThreshold; }
			set {
				if (value < 0) throw new ArgumentOutOfRangeException("LowerZoomThreshold");
				lowerZoomThreshold = value;
			}
		}


		/// <summary>
		/// Specifies the maximum zoom level where the <see cref="T:Dataweb.NShape.Layer" /> is still visible. On higher zoom levels, the layer will be hidden automatically.
		/// </summary>
		[RequiredPermission(Permission.Present)]
		public int UpperZoomThreshold {
			get { return upperZoomThreshold; }
			set {
				if (value < 0) throw new ArgumentOutOfRangeException("UpperZoomThreshold");
				upperZoomThreshold = value;
			}
		}


		#region Fields

		private LayerIds id = LayerIds.None;
		private string name = string.Empty;
		private string title = string.Empty;
		private int lowerZoomThreshold = 0;
		private int upperZoomThreshold = 5000;

		#endregion
	}


	/// <summary>
	/// Defines methods for an editable collection of layers.
	/// </summary>
	/// <status>reviewed</status>
	public interface ILayerCollection : ICollection<Layer> {

		/// <summary>
		/// Retrieve the <see cref="T:Dataweb.NShape.Layer" /> instance associated with the given <see cref="T:Dataweb.NShape.LayerIds" />.
		/// </summary>
		Layer GetLayer(LayerIds layerId);

		/// <summary>
		/// Retrieve all <see cref="T:Dataweb.NShape.Layer" /> instances associated with the given combination of <see cref="T:Dataweb.NShape.LayerIds" />.
		/// </summary>
		IEnumerable<Layer> GetLayers(LayerIds layerIds);		

		/// <summary>
		/// Retrieve the <see cref="T:Dataweb.NShape.Layer" /> instance with the given name.
		/// </summary>
		Layer FindLayer(string name);

		/// <summary>
		/// Rename the specified <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		bool RenameLayer(string previousName, string newName);

	}


	/// <summary>
	/// Holds a list of layers.
	/// </summary>
	internal class LayerCollection : ILayerCollection {

		internal LayerCollection(Diagram diagram) {
			this.diagram = diagram;
			// create an entry for each layer so that the layer can be addressed directly
			foreach (LayerIds layerId in Enum.GetValues(typeof(LayerIds))) {
				if (layerId == LayerIds.None || layerId == LayerIds.All) continue;
				layers.Add(null);
			}
		}


		#region ILayerCollection Members

		public Layer FindLayer(string name) {
			if (name == null) throw new ArgumentNullException("name");
			int cnt = layers.Count;
			for (int i = 0; i < cnt; ++i) {
				if (layers[i] != null && string.Compare(layers[i].Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
					return layers[i];
			}
			return null;
		}


		public Layer GetLayer(LayerIds layerId) {
			int layerBit = GetLayerBit(layerId);
			if (layerBit < 0) throw new NShapeException("{0} is not a valid {1} to find.", layerId, typeof(LayerIds));
			return layers[layerBit];
		}


		public IEnumerable<Layer> GetLayers(LayerIds layerId) {
			foreach (int layerBit in GetLayerBits(layerId)) {
				if (layerBit == -1) continue;
				if (layers[layerBit] != null)
					yield return layers[layerBit];
			}
		}


		public bool RenameLayer(string previousName, string newName) {
			if (previousName == null) throw new ArgumentNullException("previousName");
			if (newName == null) throw new ArgumentNullException("newName");
			Layer layer = FindLayer(newName);
			if (layer != null) return false;
			else {
				layer.Name = newName;
				return true;
			}
		}

		#endregion


		#region ICollection<Layer> Members

		public void Add(Layer item) {
			if (item == null) throw new ArgumentNullException("item");
			for (int i = 0; i < layers.Count; ++i) {
				if (layers[i] == null) {
					item.Id = (LayerIds)Math.Pow(2, i);
					layers[i] = item;
					++layerCount;
					break;
				}
			}
			Debug.Assert(item.Id != LayerIds.None);
		}

		public void Clear() {
			for (int i = 0; i < layers.Count; ++i)
				layers[i] = null;
			layerCount = 0;
		}

		public bool Contains(Layer item) {
			return layers.Contains(item);
		}

		public void CopyTo(Layer[] array, int arrayIndex) {
			if (array == null) throw new ArgumentNullException("array");
			layers.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return layerCount; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(Layer item) {
			if (item == null) throw new ArgumentNullException("item");
			int layerBit = GetLayerBit(item.Id);
			if (layerBit < 0) return false;
			Debug.Assert(item.Id == layers[layerBit].Id);
			layers[layerBit] = null;
			return true;
		}

		#endregion


		#region IEnumerable<Layer> Members

		public IEnumerator<Layer> GetEnumerator() {
			return Enumerator.Create(layers);
		}

		#endregion


		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return layers.GetEnumerator();
		}

		#endregion


		#region Methods (private)

		private int GetLayerBit(LayerIds layerId) {
			foreach (int layerBit in GetLayerBits(layerId))
				return layerBit;
			return -1;
		}


		private IEnumerable<int> GetLayerBits(LayerIds layerIds) {
			if (layerIds == LayerIds.None) yield break;
			int bitNo = 0;
			foreach (LayerIds id in Enum.GetValues(typeof(LayerIds))) {
				if (id == LayerIds.None || id == LayerIds.All) continue;
				if ((layerIds & id) != 0)
					yield return bitNo;
				++bitNo;
			}
		}

		#endregion


		private struct Enumerator : IEnumerator<Layer>, IEnumerator {

			public static Enumerator Create(List<Layer> layerList) {
			if (layerList == null) throw new ArgumentNullException("layerList");
				Enumerator result = Enumerator.Empty;
				result.layerList = layerList;
				result.layerCount = layerList.Count;
				result.currentIdx = -1;
				result.currentLayer = null;
				return result;
			}


			public static readonly Enumerator Empty;


			public Enumerator(List<Layer> layerList) {
				if (layerList == null) throw new ArgumentNullException("layerList");
				this.layerList = layerList;
				this.layerCount = layerList.Count;
				this.currentIdx = -1;
				this.currentLayer = null;
			}


			#region IEnumerator<Layer> Members

			public Layer Current { get { return currentLayer; } }

			#endregion


			#region IDisposable Members

			public void Dispose() {
				// nothing to do
			}

			#endregion


			#region IEnumerator Members

			object IEnumerator.Current { get { return currentLayer; } }

			public bool MoveNext() {
				bool result = false;
				currentLayer = null;
				while (currentIdx < layerCount - 1 && !result) {
					currentLayer = layerList[++currentIdx];
					if (currentLayer != null) result = true;
				}
				return result;
			}

			public void Reset() {
				currentIdx = -1;
				currentLayer = null;
			}

			#endregion


			static Enumerator() {
				Empty.layerList = null;
				Empty.layerCount = 0;
				Empty.currentIdx = -1;
				Empty.currentLayer = null;
			}

			#region Fields
			private List<Layer> layerList;
			private int layerCount;
			private int currentIdx;
			private Layer currentLayer;
			#endregion
		}


		#region Fields
		private List<Layer> layers = new List<Layer>(31);
		private int layerCount = 0;
		private Diagram diagram = null;
		#endregion
	}

	#endregion


	/// <summary>
	/// Read/write shape collection owned by a diagram.
	/// </summary>
	internal class DiagramShapeCollection : ShapeCollection {

		/// <override></override>
		public override void NotifyChildMoving(Shape shape) {
			base.NotifyChildMoving(shape);
			CheckOwnerboundsUpdateNeeded(shape);
			++shapeCounter;
		}


		/// <override></override>
		public override void NotifyChildMoved(Shape shape) {
			base.NotifyChildMoved(shape);
			CheckOwnerboundsUpdateNeeded(shape);
			--shapeCounter;
			if (shapeCounter == 0) DoUpdateOwnerBounds();
		}


		/// <override></override>
		public override void NotifyChildResizing(Shape shape) {
			base.NotifyChildResizing(shape);
			CheckOwnerboundsUpdateNeeded(shape);
			++shapeCounter;
		}


		/// <override></override>
		public override void NotifyChildResized(Shape shape) {
			base.NotifyChildResized(shape);
			CheckOwnerboundsUpdateNeeded(shape);
			--shapeCounter;
			if (shapeCounter == 0) DoUpdateOwnerBounds();
		}


		/// <override></override>
		public override void NotifyChildRotating(Shape shape) {
			base.NotifyChildRotating(shape);
			CheckOwnerboundsUpdateNeeded(shape);
			++shapeCounter;
		}


		/// <override></override>
		public override void NotifyChildRotated(Shape shape) {
			base.NotifyChildRotated(shape);
			CheckOwnerboundsUpdateNeeded(shape);
			--shapeCounter;
			if (shapeCounter == 0) DoUpdateOwnerBounds();
		}


		internal DiagramShapeCollection(Diagram owner)
			: this(owner, 1000) {
		}


		internal DiagramShapeCollection(Diagram owner, int capacity)
			: base(capacity) {
			if (owner == null) throw new ArgumentNullException("owner");
			this.owner = owner;
		}


		internal DiagramShapeCollection(Diagram owner, IEnumerable<Shape> collection)
			: this(owner, (collection is ICollection) ? ((ICollection)collection).Count : 0) {
			AddRangeCore(collection);
		}


		internal Diagram Owner {
			get { return owner; }
		}


		/// <override></override>
		protected override void AddRangeCore(IEnumerable<Shape> collection) {
			if (collection is ICollection) shapeCounter = ((ICollection)collection).Count;
			else foreach (Shape s in collection) ++shapeCounter;
			base.AddRangeCore(collection);
		}


		/// <override></override>
		protected override bool RemoveRangeCore(IEnumerable<Shape> collection) {
			if (collection is ICollection) shapeCounter = ((ICollection)collection).Count;
			else foreach (Shape s in collection) ++shapeCounter;
			return base.RemoveRangeCore(collection);
		}


		/// <override></override>
		protected override void ReplaceRangeCore(IEnumerable<Shape> oldShapes, IEnumerable<Shape> newShapes) {
			if (oldShapes is ICollection) shapeCounter = ((ICollection)oldShapes).Count;
			else foreach (Shape s in oldShapes) ++shapeCounter;
			base.ReplaceRangeCore(oldShapes, newShapes);
		}


		/// <override></override>
		protected override int InsertCore(int index, Shape shape) {
			int result = base.InsertCore(index, shape);
			shape.Diagram = owner;
			shape.DisplayService = owner.DisplayService;
			shape.Invalidate();

			CheckOwnerboundsUpdateNeeded(shape);
			if (shapeCounter > 0) --shapeCounter;
			if (shapeCounter == 0) DoUpdateOwnerBounds();

			return result;
		}


		/// <override></override>
		protected override void ReplaceCore(Shape oldShape, Shape newShape) {
			base.ReplaceCore(oldShape, newShape);
			oldShape.Diagram = null;
			oldShape.Invalidate();
			oldShape.DisplayService = null;
			newShape.Diagram = owner;
			newShape.DisplayService = owner.DisplayService;
			newShape.Invalidate();

			CheckOwnerboundsUpdateNeeded(oldShape);
			CheckOwnerboundsUpdateNeeded(newShape);
			if (shapeCounter > 0) --shapeCounter;
			if (shapeCounter == 0) DoUpdateOwnerBounds();
		}


		/// <override></override>
		protected override bool RemoveCore(Shape shape) {
			bool result = base.RemoveCore(shape);
			shape.Invalidate();
			shape.DisplayService = null;
			shape.Diagram = null;

			CheckOwnerboundsUpdateNeeded(shape);
			if (shapeCounter > 0) --shapeCounter;
			if (shapeCounter == 0) DoUpdateOwnerBounds();

			return result;
		}


		/// <override></override>
		protected override void ClearCore() {
			for (int i = shapes.Count - 1; i >= 0; --i) {
				CheckOwnerboundsUpdateNeeded(shapes[i]);
				shapes[i].Invalidate();
				shapes[i].DisplayService = null;
			}
			base.ClearCore();
			DoUpdateOwnerBounds();
		}


		private void CheckOwnerboundsUpdateNeeded(Shape shape) {
			if (!ownerBoundsUpdateNeeded) {
				Rectangle shapeBounds = shape.GetBoundingRectangle(true);
				if (shapeBounds.Left < 0 || owner.Width < shapeBounds.Right
					|| shapeBounds.Top < 0 || owner.Height < shapeBounds.Bottom)
					ownerBoundsUpdateNeeded = true;
			}
		}


		private void DoUpdateOwnerBounds() {
			Debug.Assert(shapeCounter == 0);
			if (ownerBoundsUpdateNeeded) {
				if (owner != null) owner.NotifyBoundsChanged();
				ownerBoundsUpdateNeeded = false;
			}
		}


		#region Fields
		private Diagram owner = null;
		private int shapeCounter = 0;
		private bool ownerBoundsUpdateNeeded;
		#endregion
	}


	/// <summary>
	/// Displays shapes in layers.
	/// </summary>
	[TypeDescriptionProvider(typeof(TypeDescriptionProviderDg))]
	public sealed class Diagram : IEntity {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Diagram" />.
		/// </summary>
		public Diagram(string name) {
			if (name == null) throw new ArgumentNullException("name");
			this.name = name;
			diagramShapes = new DiagramShapeCollection(this, expectedShapes);
			layers = new LayerCollection(this);
			// A new diagram has no layers.

			// Set size to DIN A4
			using (Graphics gfx = Graphics.FromHwnd(IntPtr.Zero)) {
				Width = (int)Math.Round((gfx.DpiX * (1/25.4f)) * 210);
				Height = (int)Math.Round((gfx.DpiY * (1/25.4f)) * 297);
			}
		}


		#region [Public] Properties

		/// <summary>
		/// Culture invariant name.
		/// </summary>
		[Category("General")]
		[Description("The name of the diagram.")]
		[RequiredPermission(Permission.ModifyData)]
		public string Name {
			get { return name; }
			set { name = value ?? string.Empty; }
		}


		/// <summary>
		/// Culture depending title.
		/// </summary>
		[Category("General")]
		[Description("The displayed text of the diagram.")]
		[RequiredPermission(Permission.Present)]
		public string Title {
			get { return string.IsNullOrEmpty(title) ? name : title; }
			set {
				if (value == name || string.IsNullOrEmpty(value))
					title = null;
				else title = value;
			}
		}


		/// <summary>
		/// Width of diagram in pixels.
		/// </summary>
		[Category("Layout")]
		[Description("The width of the diagram.")]
		[RequiredPermission(Permission.Layout)]
		public int Width {
			get { return size.Width; }
			set {
				if (Size.Width != value) {
					if (displayService != null)
						displayService.Invalidate(0, 0, Width, Height);
					if (value <= 0) size.Width = 1;
					else size.Width = value;
					if (displayService != null) {
						displayService.NotifyBoundsChanged();
						displayService.Invalidate(0, 0, Width, Height);
					}
				}
			}
		}


		/// <summary>
		/// Height of diagram in pixels.
		/// </summary>
		[Category("Layout")]
		[Description("The height of the diagram.")]
		[RequiredPermission(Permission.Layout)]
		public int Height {
			get { return size.Height; }
			set {
				if (size.Height != value) {
					if (displayService != null)
						displayService.Invalidate(0, 0, Width, Height);
					if (value <= 0) size.Height = 1;
					else size.Height = value;
					if (displayService != null) {
						displayService.NotifyBoundsChanged();
						displayService.Invalidate(0, 0, Width, Height);
					}
				}
			}
		}


		/// <summary>
		/// Size of diagram in pixels.
		/// </summary>
		[Browsable(false)]
		public Size Size {
			get { return size; }
			set {
				if (displayService != null)
					displayService.Invalidate(0, 0, Width, Height);
				size = value;
				if (displayService != null)
					displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// Background color of the diagram.
		/// </summary>
		[Category("Appearance")]
		[Description("The background color of the diagram.")]
		[RequiredPermission(Permission.Present)]
		public Color BackgroundColor {
			get { return backColor; }
			set { 
				backColor = value;
				if (colorBrush != null) {
					colorBrush.Dispose();
					colorBrush = null;
				}
				if (displayService != null)
					displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// Second color of background gradient.
		/// </summary>
		[Category("Appearance")]
		[Description("The second color of the diagram's color gradient.")]
		[RequiredPermission(Permission.Present)]
		public Color BackgroundGradientColor {
			get { return targetColor; }
			set { 
				targetColor = value;
				if (colorBrush != null) {
					colorBrush.Dispose();
					colorBrush = null;
				}
				if (displayService != null)
					displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// Background image of diagram.
		/// </summary>
		[Category("Appearance")]
		[Description("The background image of the diagram.")]
		[Editor("Dataweb.NShape.WinFormsUI.NamedImageUITypeEditor, Dataweb.NShape.WinFormsUI", typeof(UITypeEditor))]
		[RequiredPermission(Permission.Present)]
		public NamedImage BackgroundImage {
			get { return backImage; }
			set {
				backImage = value;
				InvalidateDrawCache();
				if (displayService != null) displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// Image layout of background image.
		/// </summary>
		[Category("Appearance")]
		[Description("The display mode of the diagram's background image.")]
		[RequiredPermission(Permission.Present)]
		public ImageLayoutMode BackgroundImageLayout {
			get { return imageLayout; }
			set { 
				imageLayout = value;
				InvalidateDrawCache();
				if (displayService != null) displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// Gamma correction factor for the background image.
		/// </summary>
		[Category("Appearance")]
		[Description("Gamma correction for the diagram's background image.")]
		[RequiredPermission(Permission.Present)]
		public float BackgroundImageGamma {
			get { return imageGamma; }
			set {
				if (value <= 0) throw new ArgumentOutOfRangeException("Value has to be greater 0.");
				imageGamma = value;
				InvalidateDrawCache();
				if (displayService != null) displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// Specifies if the background image should be displayed as gray scale image.
		/// </summary>
		[Category("Appearance")]
		[Description("Specifies if the diagram's background image is drawn as gray scale image instead.")]
		[RequiredPermission(Permission.Present)]
		public bool BackgroundImageGrayscale {
			get { return imageGrayScale; }
			set {
				imageGrayScale = value;
				InvalidateDrawCache();
				if (displayService != null) displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// Transparency of the background image in percentage.
		/// </summary>
		[Category("Appearance")]
		[Description("Specifies the transparency in percentage for the diagram's background image.")]
		[RequiredPermission(Permission.Present)]
		public byte BackgroundImageTransparency {
			get { return imageTransparency; }
			set {
				if (value < 0 || value > 100) throw new ArgumentOutOfRangeException("The value has to be between 0 and 100.");
				imageTransparency = value;
				InvalidateDrawCache();
				if (displayService != null) displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// The specified color of the background image will be transparent.
		/// </summary>
		[Category("Appearance")]
		[Description("Specifies a color that will be transparent in the diagram's background image.")]
		[RequiredPermission(Permission.Present)]
		public Color BackgroundImageTransparentColor {
			get { return imageTransparentColor; }
			set {
				imageTransparentColor = value;
				InvalidateDrawCache();
				if (displayService != null) displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// Specifies the display service to use for this diagram.
		/// </summary>
		[Browsable(false)]
		public IDisplayService DisplayService {
			get { return displayService; }
			set {
				if (displayService != value) {
					displayService = value;
					diagramShapes.SetDisplayService(displayService);
				}
				if (displayService != null) displayService.Invalidate(0, 0, Width, Height);
			}
		}


		/// <summary>
		/// Provides access to the diagram layers.
		/// </summary>
		[Browsable(false)]
		public ILayerCollection Layers {
		   get { return layers; }
		}


		/// <summary>
		/// Provides access to the diagram shapes.
		/// </summary>
		[Browsable(false)]
		public IShapeCollection Shapes {
			get { return diagramShapes; }
		}

		#endregion


		#region [Public] Methods: Layer management

		/// <summary>
		/// Gets all <see cref="T:Dataweb.NShape.LayerIds" /> the given <see cref="T:Dataweb.NShape.Advanced.NShape" /> is part of.
		/// </summary>
		public LayerIds GetShapeLayers(Shape shape) {
			if (shape == null) throw new ArgumentNullException("shape");
			return shape.Layers;
		}
		

		/// <summary>
		/// Associates the given <see cref="T:Dataweb.NShape.Advanced.Shape" /> to all specified layers.
		/// </summary>
		public void AddShapeToLayers(Shape shape, LayerIds layerIds) {
			if (shape == null) throw new ArgumentNullException("shape");
			shape.Layers |= layerIds;
			shape.Invalidate();
		}


		/// <summary>
		/// Associates the given collection of <see cref="T:Dataweb.NShape.Advanced.Shape" /> to all specified layers.
		/// </summary>
		public void AddShapesToLayers(IEnumerable<Shape> shapes, LayerIds layerIds) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			foreach (Shape shape in shapes) {
				shape.Layers |= layerIds;
				shape.Invalidate();
			}
		}


		/// <summary>
		/// Disociates the given <see cref="T:Dataweb.NShape.Advanced.Shape" /> to all specified layers.
		/// </summary>
		public void RemoveShapeFromLayers(Shape shape, LayerIds layerIds) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (layerIds == LayerIds.None) return;
			if (layerIds == LayerIds.All)
				shape.Layers = LayerIds.None;
			else shape.Layers ^= (shape.Layers & layerIds);
			shape.Invalidate();
		}


		/// <summary>
		/// Disociates the given collection of <see cref="T:Dataweb.NShape.Advanced.Shape" /> to all specified layers.
		/// </summary>
		public void RemoveShapesFromLayers(IEnumerable<Shape> shapes, LayerIds layerIds) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			foreach (Shape shape in shapes) {
				shape.Layers ^= (shape.Layers & layerIds);
				shape.Invalidate();
			}
		}


		/// <summary>
		/// Delete all content of this <see cref="T:Dataweb.NShape.Diagram" />.
		/// </summary>
		public void Clear() {
			diagramShapes.Clear();
			layers.Clear();
		}

		#endregion


		#region [Public] Methods: Drawing and painting

		/// <summary>
		/// Exports the contents of the diagram to an image of the given format.
		/// </summary>
		/// <param name="imageFormat">Specifies the format of the graphics file.</param>
		public Image CreateImage(ImageFileFormat imageFormat) {
			return CreateImage(imageFormat, null, 0, false, Color.White, -1);
		}


		/// <summary>
		/// Exports the part of the diagram that encloses all given shapes to an image of the given format.
		/// Pass null/Nothing for Parameter shapes in order to expor the whole diagram area.
		/// </summary>
		/// <param name="imageFormat">Specifies the format of the graphics file.</param>
		/// <param name="shapes">The shapes that should be drawn. If null/Nothing, the whole diagram area will be exported.</param>
		public Image CreateImage(ImageFileFormat imageFormat, IEnumerable<Shape> shapes) {
			return CreateImage(imageFormat, shapes, 0, false, Color.White, -1);
		}


		/// <summary>
		/// Exports the part of the diagram that encloses all given shapes to an image of the given format.
		/// Pass null/Nothing for Parameter shapes in order to expor the whole diagram area.
		/// </summary>
		/// <param name="imageFormat">Specifies the format of the graphics file.</param>
		/// <param name="shapes">The shapes that should be drawn. If null/Nothing, the whole diagram area will be exported.</param>
		/// <param name="withBackground">Specifies wether the diagram's background should be exported to the graphics file.</param>
		public Image CreateImage(ImageFileFormat imageFormat, IEnumerable<Shape> shapes, bool withBackground) {
			return CreateImage(imageFormat, shapes, 0, withBackground, Color.White, -1);
		}


		/// <summary>
		/// Exports the part of the diagram that encloses all given shapes to an image of the given format.
		/// Pass null/Nothing for Parameter shapes in order to expor the whole diagram area.
		/// </summary>
		/// <param name="imageFormat">Specifies the format of the graphics file.</param>
		/// <param name="shapes">The shapes that should be drawn. If null/Nothing, the whole diagram area will be exported.</param>
		/// <param name="margin">Specifies the thickness of the margin around the exported diagram area.</param>
		public Image CreateImage(ImageFileFormat imageFormat, IEnumerable<Shape> shapes, int margin) {
			return CreateImage(imageFormat, shapes, margin, false, Color.White, -1);
		}


		/// <summary>
		/// Exports the part of the diagram that encloses all given shapes (plus margin on each side) to an image of the given format.
		/// Pass null/Nothing for Parameter shapes in order to expor the whole diagram area.
		/// </summary>
		/// <param name="imageFormat">Specifies the format of the graphics file.</param>
		/// <param name="shapes">The shapes that should be drawn. If null/Nothing, the whole diagram area will be exported.</param>
		/// <param name="margin">Specifies the thickness of the margin around the exported diagram area.</param>
		/// <param name="withBackground">Specifies wether the diagram's background should be exported to the graphics file.</param>
		/// <param name="backgroundColor">Specifies a color for the exported image's background. 
		/// If the diagram is exported with background, the diagram's background will be drawn over the specified background color.</param>
		public Image CreateImage(ImageFileFormat imageFormat, IEnumerable<Shape> shapes, int margin, bool withBackground, Color backgroundColor) {
			return CreateImage(imageFormat, shapes, margin, withBackground, backgroundColor, -1);
		}
		
		
		/// <summary>
		/// Exports the part of the diagram that encloses all given shapes (plus margin on each side) to an image of the given format.
		/// Pass null/Nothing for Parameter shapes in order to expor the whole diagram area.
		/// </summary>
		/// <param name="imageFormat">Specifies the format of the graphics file.</param>
		/// <param name="shapes">The shapes that should be drawn. If null/Nothing, the whole diagram area will be exported.</param>
		/// <param name="margin">Specifies the thickness of the margin around the exported diagram area.</param>
		/// <param name="withBackground">Specifies wether the diagram's background should be exported to the graphics file.</param>
		/// <param name="backgroundColor">Specifies a color for the exported image's background. 
		/// If the diagram is exported with background, the diagram's background will be drawn over the specified background color.</param>
		/// <param name="dpi">Specifies the resolution for the export file. Only applies to pixel based image file formats.</param>
		public Image CreateImage(ImageFileFormat imageFormat, IEnumerable<Shape> shapes, int margin, bool withBackground, Color backgroundColor, int dpi) {
			Image result = null;
			
			// Get/Create info graphics
			bool disposeInfoGfx;
			Graphics infoGraphics;
			if (DisplayService != null) {
				infoGraphics = DisplayService.InfoGraphics;
				disposeInfoGfx = false;
			} else {
				infoGraphics = Graphics.FromHwnd(IntPtr.Zero);
				disposeInfoGfx = true;
			}

			try {
				// If dpi value is not valid, get current dpi from display service
				if (dpi <= 0) 
					dpi = (int)Math.Round((infoGraphics.DpiX + infoGraphics.DpiY) / 2f);

				// Get bounding rectangle around the given shapes
				Rectangle imageBounds = Rectangle.Empty;
				if (shapes == null) {
					imageBounds.X = imageBounds.Y = 0;
					imageBounds.Width = Width;
					imageBounds.Height = Height;
				} else {
					int left, top, right, bottom;
					left = top = int.MaxValue;
					right = bottom = int.MinValue;
					// Calculate the bounding rectangle of the given shapes
					Rectangle boundingRect = Rectangle.Empty;
					foreach (Shape shape in shapes) {
						boundingRect = shape.GetBoundingRectangle(true);
						if (boundingRect.Left < left) left = boundingRect.Left;
						if (boundingRect.Top < top) top = boundingRect.Top;
						if (boundingRect.Right > right) right = boundingRect.Right;
						if (boundingRect.Bottom > bottom) bottom = boundingRect.Bottom;
					}
					if (Geometry.IsValid(left, top, right, bottom))
						imageBounds = Rectangle.FromLTRB(left, top, right, bottom);
				}
				imageBounds.Inflate(margin, margin);
				imageBounds.Width += 1;
				imageBounds.Height += 1;

				bool originalQualitySetting = this.HighQualityRendering;
				HighQualityRendering = true;
				UpdateBrushes();

				float scaleX = 1, scaleY = 1;
				switch (imageFormat) {
					case ImageFileFormat.Svg:
						throw new NotImplementedException();

					case ImageFileFormat.Emf:
					case ImageFileFormat.EmfPlus:
						// Create MetaFile and graphics context
						IntPtr hdc = infoGraphics.GetHdc();
						try {
							Rectangle bounds = Rectangle.Empty;
							bounds.Size = imageBounds.Size;
							result = new Metafile(hdc, bounds, MetafileFrameUnit.Pixel,
												imageFormat == ImageFileFormat.Emf ? EmfType.EmfOnly : EmfType.EmfPlusDual,
												Name);
						} finally {
							infoGraphics.ReleaseHdc(hdc);
						}
						break;

					case ImageFileFormat.Bmp:
					case ImageFileFormat.Gif:
					case ImageFileFormat.Jpeg:
					case ImageFileFormat.Png:
					case ImageFileFormat.Tiff:
						if (dpi > 0 && dpi != infoGraphics.DpiX || dpi != infoGraphics.DpiY) {
							scaleX = dpi / infoGraphics.DpiX;
							scaleY = dpi / infoGraphics.DpiY;
							result = new Bitmap(Math.Max(1, (int)Math.Round(scaleX * imageBounds.Width)),
														Math.Max(1, (int)Math.Round(scaleY * imageBounds.Height)));
							((Bitmap)result).SetResolution(dpi, dpi);
						} else result = new Bitmap(imageBounds.Width, imageBounds.Height);
						break;

					default:
						throw new NShapeUnsupportedValueException(typeof(ImageFileFormat), imageFormat);
				}

				// Draw diagram
				using (Graphics gfx = Graphics.FromImage(result)) {
					GdiHelpers.ApplyGraphicsSettings(gfx, RenderingQuality.MaximumQuality);

					// Fill background with background color
					if (backgroundColor.A < 255) {
						// For image formats that do not support transparency, fill background with the RGB part of 
						// the given backgropund color
						if (imageFormat == ImageFileFormat.Bmp || imageFormat == ImageFileFormat.Jpeg)
							gfx.Clear(Color.FromArgb(255, backgroundColor));
						else if (backgroundColor.A > 0) gfx.Clear(backgroundColor);
						// Skip filling background for meta files if transparency is 100%: 
						// Filling Background with Color.Transparent causes graphical glitches with many applications
					} else gfx.Clear(backgroundColor);

					// Transform graphics (if necessary)
					gfx.TranslateTransform(-imageBounds.X, -imageBounds.Y, MatrixOrder.Prepend);
					if (scaleX != 1 || scaleY != 1) gfx.ScaleTransform(scaleX, scaleY, MatrixOrder.Append);

					// Draw diagram background
					if (withBackground) DrawBackground(gfx, imageBounds);
					// Draw diagram shapes
					if (shapes == null) {
						foreach (Shape shape in diagramShapes.BottomUp) shape.Draw(gfx);
					} else {
						// Add shapes to ShapeCollection (in order to maintain zOrder while drawing)
						int cnt = (shapes is ICollection) ? ((ICollection)shapes).Count : -1;
						ShapeCollection shapeCollection = new ShapeCollection(cnt);
						foreach (Shape s in shapes) {
							// Sort out duplicate references to shapes (as they can occur in the result of Diagram.FindShapes())
							if (shapeCollection.Contains(s)) continue;
							shapeCollection.Add(s, s.ZOrder);
						}
						// Draw shapes
						foreach (Shape shape in shapeCollection.BottomUp) 
							shape.Draw(gfx);
						shapeCollection.Clear();
					}
					// Reset transformation
					gfx.ResetTransform();
				}

				HighQualityRendering = originalQualitySetting;
				UpdateBrushes();

				return result;
			} finally {
				if (disposeInfoGfx) 
					GdiHelpers.DisposeObject(ref infoGraphics);
			}
		}


		/// <summary>
		/// Draws the diagram background.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="clipRectangle"></param>
		public void DrawBackground(Graphics graphics, Rectangle clipRectangle) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			Rectangle bounds = Rectangle.Empty;
			bounds.X = Math.Max(0, clipRectangle.X);
			bounds.Y = Math.Max(0, clipRectangle.Y);
			bounds.Width = Math.Min(clipRectangle.Right, Width) - bounds.X;
			bounds.Height = Math.Min(clipRectangle.Bottom, Height) - bounds.Y;

			// draw diagram background color
			UpdateBrushes();
			//graphics.FillRectangle(colorBrush, clipRectangle);
			graphics.FillRectangle(colorBrush, bounds);

			// draw diagram background image
			if (!NamedImage.IsNullOrEmpty(backImage)) {
				Rectangle diagramBounds = Rectangle.Empty;
				diagramBounds.Width = Width;
				diagramBounds.Height = Height;
				if (imageAttribs == null) imageAttribs = GdiHelpers.GetImageAttributes(imageLayout, imageGamma, imageTransparency, imageGrayScale, false, imageTransparentColor);
				if (backImage.Image is Metafile)
					GdiHelpers.DrawImage(graphics, backImage.Image, imageAttribs, imageLayout, diagramBounds, diagramBounds);
				else {
					if (imageBrush == null) imageBrush = GdiHelpers.CreateTextureBrush(backImage.Image, imageAttribs);
					Point center = Point.Empty;
					center.Offset(Width / 2, Height / 2);
					GdiHelpers.TransformTextureBrush(imageBrush, imageLayout, diagramBounds, center, 0);
					graphics.FillRectangle(imageBrush, bounds);
				}
			}
		}


		/// <summary>
		/// Draws the diagram shapes.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="layers"></param>
		/// <param name="clipRectangle"></param>
		public void DrawShapes(Graphics graphics, LayerIds layers, Rectangle clipRectangle) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			int x = clipRectangle.X;
			int y = clipRectangle.Y;
			int width = clipRectangle.Width;
			int height = clipRectangle.Height;
			
			foreach (Shape shape in diagramShapes.BottomUp) {
				// Paint shape if it intersects with the clipping area
				if (shape.Layers != LayerIds.None && (shape.Layers & layers) == 0) continue;
				if (Geometry.RectangleIntersectsWithRectangle(shape.GetBoundingRectangle(false), x, y, width, height))
					shape.Draw(graphics);
			}
		}
		
		#endregion


		/// <summary>
		/// Specifies whether the diagram is rendered with high visual quality. 
		/// This property is typically set by the diagram presenter.
		/// </summary>
		[Browsable(false)]
		public bool HighQualityRendering {
			get { return highQualityRendering; }
			set {
				highQualityRendering = value;
				if (colorBrush != null) {
					colorBrush.Dispose();
					colorBrush = null;
				}
			}
		}
		
		
		internal void NotifyBoundsChanged() {
			if (displayService != null) displayService.NotifyBoundsChanged();
		}
		
		
		#region IEntity Members (Explicit implementation)

		/// <summary>
		/// The entity type name of <see cref="T:Dataweb.NShape.Diagram" />.
		/// </summary>
		public static string EntityTypeName {
			get { return entityTypeName; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Diagram" />.
		/// </summary>
		public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
			yield return new EntityFieldDefinition("Name", typeof(string));
			if (version > 2) yield return new EntityFieldDefinition("Title", typeof(string));
			yield return new EntityFieldDefinition("Width", typeof(int));
			yield return new EntityFieldDefinition("Height", typeof(int));
			yield return new EntityFieldDefinition("BackgroundColor", typeof(Color));
			yield return new EntityFieldDefinition("BackgroundGradientEndColor", typeof(Color));
			yield return new EntityFieldDefinition("BackgroundImageFileName", typeof(string));
			yield return new EntityFieldDefinition("BackgroundImage", typeof(Image));
			yield return new EntityFieldDefinition("ImageLayout", typeof(byte));
			yield return new EntityFieldDefinition("ImageGamma", typeof(float));
			yield return new EntityFieldDefinition("ImageTransparency", typeof(byte));
			yield return new EntityFieldDefinition("ImageGrayScale", typeof(bool));
			yield return new EntityFieldDefinition("ImageTransparentColor", typeof(int));

			yield return new EntityInnerObjectsDefinition("Layers", "Core.Layer",
				new string[] { "Id", "Name", "Title", "LowerVisibilityThreshold", "UpperVisibilityThreshold" },
				new Type[] { typeof(int), typeof(string), typeof(string), typeof(int), typeof(int) });
		}


		[Category("General")]
		object IEntity.Id {
			get { return id; }
		}


		void IEntity.AssignId(object id) {
			if (id == null)
				throw new ArgumentNullException("id");
			if (this.id != null)
				throw new InvalidOperationException(string.Format("{0} has already a id.", GetType().Name));
			this.id = id;
		}


		void IEntity.LoadFields(IRepositoryReader reader, int version) {
			name = reader.ReadString();
			if (version > 2) title = reader.ReadString();
			size.Width = reader.ReadInt32();
			size.Height = reader.ReadInt32();
			backColor = Color.FromArgb(reader.ReadInt32());
			targetColor = Color.FromArgb(reader.ReadInt32());
			string imgName = reader.ReadString();
			Image img = reader.ReadImage();
			if (img != null) backImage = new NamedImage(img, imgName);
			imageLayout = (ImageLayoutMode)reader.ReadByte();
			imageGamma = reader.ReadFloat();
			imageTransparency = reader.ReadByte();
			imageGrayScale = reader.ReadBool();
			imageTransparentColor = Color.FromArgb(reader.ReadInt32());
		}


		void IEntity.LoadInnerObjects(string propertyName, IRepositoryReader reader, int version) {
			Debug.Assert(propertyName == "Layers");
			Debug.Assert(layers.Count == 0);
			reader.BeginReadInnerObjects();
			while (reader.BeginReadInnerObject()) {
				int id = reader.ReadInt32();
				string name = reader.ReadString();
				Layer l = new Layer(name);
				l.Id = (LayerIds)id;
				l.Title = reader.ReadString();
				l.LowerZoomThreshold = reader.ReadInt32();
				l.UpperZoomThreshold = reader.ReadInt32();
				reader.EndReadInnerObject();
				layers.Add(l);
			}
			reader.EndReadInnerObjects();
		}


		void IEntity.SaveFields(IRepositoryWriter writer, int version) {
			writer.WriteString(name);
			if (version > 2) writer.WriteString(title);
			writer.WriteInt32(size.Width);
			writer.WriteInt32(size.Height);
			writer.WriteInt32(BackgroundColor.ToArgb());
			writer.WriteInt32(BackgroundGradientColor.ToArgb());
			if (NamedImage.IsNullOrEmpty(backImage)) {
				writer.WriteString(string.Empty);
				writer.WriteImage(null);
			} else {
				writer.WriteString(backImage.Name);
				object imgTag = backImage.Image.Tag;
				backImage.Image.Tag = backImage.Name;
				writer.WriteImage(backImage.Image);
				backImage.Image.Tag = imgTag;
			}
			writer.WriteByte((byte)imageLayout);
			writer.WriteFloat(imageGamma);
			writer.WriteByte(imageTransparency);
			writer.WriteBool(imageGrayScale);
			writer.WriteInt32(imageTransparentColor.ToArgb());
		}


		void IEntity.SaveInnerObjects(string propertyName, IRepositoryWriter writer, int version) {
			Debug.Assert(propertyName == "Layers");
			writer.BeginWriteInnerObjects();
			foreach (Layer l in layers) {
				writer.BeginWriteInnerObject();
				writer.WriteInt32((int)l.Id);
				writer.WriteString(l.Name);
				writer.WriteString(l.Title);
				writer.WriteInt32(l.LowerZoomThreshold);
				writer.WriteInt32(l.UpperZoomThreshold);
				writer.EndWriteInnerObject();
			}
			writer.EndWriteInnerObjects();
		}


		void IEntity.Delete(IRepositoryWriter writer, int version) {
			foreach (EntityPropertyDefinition pi in GetPropertyDefinitions(version)) {
				if (pi is EntityInnerObjectsDefinition)
					writer.DeleteInnerObjects();
			}
		}

		#endregion


		#region [Private] Methods

		private void InvalidateDrawCache() {
			if (imageAttribs != null) {
				imageAttribs.Dispose();
				imageAttribs = null;
			}
			if (imageBrush != null) {
				imageBrush.Dispose();
				imageBrush = null;
			}
		}


		private void UpdateBrushes() {
			if (colorBrush == null) {
				if (BackgroundGradientColor != BackgroundColor && highQualityRendering) {
					colorBrushBounds.Location = Point.Empty;
					colorBrushBounds.Width = 100;
					colorBrushBounds.Height = 100;
					colorBrush = new LinearGradientBrush(colorBrushBounds, BackgroundGradientColor, BackgroundColor, 45);
				} else colorBrush = new SolidBrush(BackgroundColor);
			}
			if (colorBrush is LinearGradientBrush && Size != colorBrushBounds.Size) {
				LinearGradientBrush gradientBrush = (LinearGradientBrush)colorBrush;
				colorBrushBounds.Location = Point.Empty; 
				colorBrushBounds.Width = Width;
				colorBrushBounds.Height = Height;
				PointF center = PointF.Empty;
				center.X = colorBrushBounds.X + (colorBrushBounds.Width / 2f);
				center.Y = colorBrushBounds.Y + (colorBrushBounds.Height / 2f);
				GdiHelpers.TransformLinearGradientBrush(gradientBrush, 45, colorBrushBounds, center, 0);
			}
		}

		#endregion


		#region Fields

		/// <summary>Defines the cell size of the diagram's spatial index.</summary>
		public const int CellSize = 100;

		private const string entityTypeName = "Core.Diagram";
		private const int expectedShapes = 10000;

		private object id;
		private string title;
		private string name;
		private IDisplayService displayService;
		private LayerCollection layers = null;
		private DiagramShapeCollection diagramShapes = null;
		private Size size = new Size(1, 1);
		// Rendering stuff
		private Color backColor = Color.WhiteSmoke;
		private Color targetColor = Color.White;
		private bool highQualityRendering = true;
		// Background image stuff
		private NamedImage backImage;
		private ImageLayoutMode imageLayout;
		private float imageGamma = 1.0f;
		private byte imageTransparency = 0;
		private bool imageGrayScale = false;
		private Color imageTransparentColor = Color.Empty;
		// Drawing and Painting stuff
		private Brush colorBrush = null;					// Brush for painting the diagram's background
		private ImageAttributes imageAttribs = null; // ImageAttributes for drawing the background image
		private TextureBrush imageBrush = null;		// Brush for painting the diagram's background image
		private Rectangle colorBrushBounds = Rectangle.Empty;
		
		// Buffers
		private List<Shape> shapeBuffer = new List<Shape>();

		#endregion
	}
}