
namespace Core.Interface.AssetBuilder
{

   /// <summary>
   /// 编译策略
   /// </summary>
   public interface IBuildStrategy
   {
       /// <summary>
       /// 编译流水线位置
       /// </summary>
       BuildProcess Process { get; }

       /// <summary>
       /// 当编译时
       /// </summary>
       /// <param name="context">上下文</param>
       void Build(IBuildContext context);
   }
}
