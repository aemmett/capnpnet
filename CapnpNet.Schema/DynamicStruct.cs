using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CapnpNet.Schema
{
  public class DynamicStruct : IDynamicMetaObjectProvider
  {
    private Struct _s;

    public DynamicMetaObject GetMetaObject(Expression parameter)
    {
      return new DynamicStructMetaObject(parameter);
    }

    public sealed class DynamicStructMetaObject : DynamicMetaObject
    {
      public DynamicStructMetaObject(Expression exp) 
        : base(exp, BindingRestrictions.Empty)
      {
      }

      public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
      {
        return base.BindGetMember(binder);
      }
    }
  }
}
