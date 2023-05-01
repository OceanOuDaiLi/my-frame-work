
/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	ShaderEnum.cs
	Author:		DaiLi.Ou
	Descriptions: Core Lit Shader Enums.
*********************************************************************/

namespace TechArtist.Editor
{
    /* 
     * Material Enum
     * 
     * https://blog.csdn.net/liquanyi007/article/details/110519612 
     * https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@10.8/manual/simple-lit-shader.html
     */

    #region Surface Options: The Surface Options Control how the Material is rendered on a screen.

    /// <summary>
    /// 表面类型：
    /// 使用此下拉菜单将不透明或透明表面类型应用于材质。
    /// 这决定了URP在那个渲染过程中渲染材质。
    /// 不透明表面类型始终完全可见，无论它们背后是什么。
    /// URP先渲染不透明材质。
    /// 透明表面类型受其背景影响，它们会根据您选择的透明表面类型而有所不同。
    /// URP在不透明对象之后的单独通道中渲染透明材质。
    /// </summary>
    public enum SurfaceType
    {
        Opaque,                         //不透明材质
        Transparent                     //透明材质
    }

    /// <summary>
    /// 渲染面：
    /// 使用此下来列表确定要渲染几何体的那一侧。
    /// Front Face:
    ///        渲染几何体的正面，剔除背面。这是默认设置
    /// 
    /// Back Face:
    ///        渲染相机正前方的几何图形，剔除几何体朝向相机的正面  =====》 渲染了模型的反面，相当于反转法线
    ///        
    /// Both:
    ///        使URP渲染几何体的两个面。这适用于小而扁平的物体，例如树叶，您可能希望两面都可见。
    /// </summary>
    public enum RenderFace
    {
        Both = 0,
        BackFace = 1,
        FrontFace = 2
    }

    /// <summary>
    /// 混合模式：
    /// 使用此下拉列表来确定URP如何通过将材质与背景像素混合来计算透明材质每个像素的颜色。
    /// Alpha(透明)：
    ///       使用材质的alpha值来更改对象的透明度。0 是完全透明的。1 是看起来完全不透明，
    ///       但在透明渲染通道期间仍会渲染材质。这对于您希望完全可见但又随着时间消退的视觉效果非常由于，例如云。
    /// 
    /// Premultiply(预乘)：
    ///       对材质应用于Alpha类似的效果，但保留反射和高光，即使您的表面是透明的。
    ///       这意味着只有反射光是可见的。例如，透明玻璃
    ///       
    /// Additive(叠加)：
    ///       在另一个表面之上向材质添加一个额外的层。这对全息图很有用
    ///       
    /// Multiply(乘)：
    ///       将材质的颜色与表面后面的颜色相乘。这会产生较暗的效果，就像您透过彩色玻璃看时一样。
    /// 
    /// 仅针对透明材质。
    /// </summary>
    public enum BlendMode
    {
        /// <summary>
        /// 透明：
        /// 使用材质的alpha值来更改对象的透明度。0 是完全透明的。1 是看起来完全不透明，
        /// 但在透明渲染通道期间仍会渲染材质。这对于您希望完全可见但又随着时间消退的视觉效果非常由于，例如云。
        /// </summary>
        Alpha = 0,
        /// <summary>
        /// 预乘：
        /// 对材质应用于Alpha类似的效果，但保留反射和高光，即使您的表面是透明的。
        /// 这意味着只有反射光是可见的。例如，透明玻璃
        /// </summary>
        Premultiply = 1,
        /// <summary>
        /// 叠加：
        /// 在另一个表面之上向材质添加一个额外的层。这对全息图很有用
        /// </summary>
        Additive = 2,
        /// <summary>
        /// 乘：
        /// 将材质的颜色与表面后面的颜色相乘。这会产生较暗的效果，就像您透过彩色玻璃看时一样。
        /// </summary>
        Multiply = 3
    }

    /// Alpha Clipping：
    ///      Makes your Material act like a Cutout Shader.Use this to create a transparent effect
    ///      with hard edges between the opaque and transparent areas.
    ///      For example,to create blades of grass.To achieve this effect,URP does not render alpha values
    ///      below the specified Threshold,which appeats when you enable Alpha Clipping.You can set the Threshold
    ///      by moving the slider,which accepts values from 0 to 1.All values above your threshold are fully opaque,
    ///      and all values below your threshold are invisible.For example,a threshold of 0.1 means that URP doesn't render
    ///      alpha values below 0.1.the default value is 0.5.
    /// 
    ///      使你的材质表现的像Coutout Shader.用于它在不透明和透明区域之间创建的具有硬边(hard edges)的透明效果。
    ///      例如，创建草叶。为实现此效果，URP不会呈现低于指定阈值的alpha值，该阈值在启用Alpha Clipping时出现。
    ///      可通过移动滑块来设置阈值，范围在0~1之间。所有高于阈值的值都是完全不透明的，所有低于阈值的值都是不可见的。
    ///      例如，阈值0.1表示URP不会呈现低于0.1的alpha值。默认值为0.5。
    /// 
    ///仅针对不透明材质。

    /// Workflow Mode:
    ///      使用此下拉菜单选择合适的纹理工作流程。金属或高光工作流。
    ///      金属或高光工作流程信息，参阅：https://docs.unity3d.com/Manual/StandardShaderMetallicVsSpecular.html
    #endregion

    #region Surface Inputs
    /*
     * The Surface Inputs describe the surface itself.
     * For example,you can use these properties to make your surface look wet,dry,rough,or smooth.
     * 
     * '表面输入'描述表面本身。例如，您可以使用这些属性使表面看起来湿润、干燥、粗糙或光滑。
     */

    /// Base Map：
    ///      Adds color to the surface,alse know as the diffuse map.To assign a Texture to the Base Map setting,
    ///      click the object picker next to it.This opens the Asset Browser,where you can select from
    ///      the Textures in your Project.Alternatively,you can use the color picker.The color next to the setting shows 
    ///      the tint on top of your assigned Texture.To assign another tint,you can click this color swatch.
    ///      If you select Transparent or Alpha Clipping under Surface Options,your Material uses the Texture's alpha channel or color.
    ///
    ///      为表面添加颜色，也被称为漫反射纹理。
    ///      您可在Surface Options下选择Transparent或Alpha Clipping，您的材质将使用纹理的Alpha通道或颜色。

    /// Specular Map：
    ///      Controls the color of your specular highlights from direct lighting,for example Directional,Point,and Spot lights.
    ///      To assign a Texture to the Specular Mapp setting,Click the object picker next to it.This opens the Asset Browser,where you 
    ///      can select from the textres in your Project,Alternatively,you can use the color picker.
    ///      In source,you can select a Texture in your Project to act as a source fro the smothness,By default,the source is the Aloha channel
    ///      for this Texture.
    ///      You can use the Smoothness slider to control the spread of highlights on the surface.0 gives a width,rough highligh.1 gives a small,sharp
    ///      highlight like glass.Values in between produce semiglossy looks.For example,0.5 produces a plastic-like glossiness.
    ///      


    /// Normal Map：
    ///      A


    /// Emission：
    ///      A


    /// Tilling：
    ///      A


    /// Offset：
    ///      A
    #endregion
}