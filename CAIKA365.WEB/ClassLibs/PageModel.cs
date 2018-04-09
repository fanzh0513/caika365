using AosuApp.AosuFramework;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace CAIKA365.WEB.ClassLibs
{
    public class PageModel
    {
        public PageModel(BaseController controller, string file)
        {
            Parameters = new DynamicParameters();
            ParamUtil aPU = new ParamUtil().SQLCmdLoadDataFromXML(file).ExecuteCmd(ADataLoader.LocalDataLoader());
            if (!aPU.IsOK())
            {
                throw new InvalidOperationException(string.Format("上下文中未匹配到合适的Model信息！ file={0}", file));
            }

            DictSetUtil aXTCS = new DictSetUtil(aPU.GetValueAsDataSet());
            Title = aXTCS.GetValue("Title");
            Description = aXTCS.GetValue("Description");
            Keywords = aXTCS.GetValue("Keywords");
            ResConfigFile = aXTCS.GetValue("ResConfigFile");

            AppDomain = controller.AppDomain;
            ResVersion = controller.ResVersion;
        }

        public string Title
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public string Keywords
        {
            get;
            private set;
        }

        public string ResConfigFile
        {
            get;
            private set;
        }

        public string AppDomain
        {
            get;
            private set;
        }

        public string ResVersion
        {
            get;
            private set;
        }

        public dynamic Parameters
        {
            get;
            private set;
        }
    }

    public delegate object DynamicMethodDelegate(dynamic sender, params object[] parameters);

    public class DynamicMethodClass
    {
        public DynamicMethodDelegate CallMethod
        {
            get;
            private set;
        }

        private DynamicMethodClass(DynamicMethodDelegate method)
        {
            CallMethod = method;
        }
    }

    public class DynamicParameters : DynamicObject
    {
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        /// <summary>  
        /// 获取属性值  
        /// </summary>  
        /// <param name="propertyName"></param>  
        /// <returns></returns>  
        public object GetPropertyValue(string propertyName)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                if (_values[propertyName] == null)
                    return "";

                return _values[propertyName];
            }

            return null;
        }

        /// <summary>  
        /// 设置属性值  
        /// </summary>  
        /// <param name="propertyName"></param>  
        /// <param name="value"></param>  
        public void SetPropertyValue(string propertyName, object value)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                _values[propertyName] = value;
            }
            else
            {
                _values.Add(propertyName, value);
            }
        }

        /// <summary>  
        /// 实现动态对象属性成员访问的方法，得到返回指定属性的值  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="result"></param>  
        /// <returns></returns>  
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetPropertyValue(binder.Name);
            return result == null ? false : true;
        }

        /// <summary>  
        /// 实现动态对象属性值设置的方法。  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetPropertyValue(binder.Name, value);
            return true;
        }

        /// <summary>  
        /// 动态对象动态方法调用时执行的实际代码  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="args"></param>  
        /// <param name="result"></param>  
        /// <returns></returns>  
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            DynamicMethodClass theDelegateObj = GetPropertyValue(binder.Name) as DynamicMethodClass;
            if (theDelegateObj == null || theDelegateObj.CallMethod == null)
            {
                result = null;
                return false;
            }

            result = theDelegateObj.CallMethod(this, args);
            return true;
        }
    }
}