using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject
{ 
    /// <summary>
    ///  
    /// </summary>
    public class DataObjectItem 
    {
        bool _isInit = false; 
        bool _isChange = false;
        string _Name;
        object _Value;
        object _OrgValue;
        Type _DataType;
        bool _Nullable = true;
        public object Value
        {
            get
            { return _Value; }
            set
            {
                object setVal = null;
                if (value != null )
                {
                    if (_DataType == typeof(double) &&
                        (value.GetType() == typeof(int) || value.GetType() == typeof(double) || value.GetType() == typeof(float) ||
                          value.GetType() == typeof(int?) || value.GetType() == typeof(double?) || value.GetType() == typeof(float?)))
                    {
                        setVal = double.Parse(value.ToString());
                    }
                    else if (_DataType == typeof(DateTime) &&
                        (value.GetType() == typeof(DateTime)))
                    {
                        setVal = value;
                    }
                    else if (_DataType == typeof(DateTime) &&
                       (value.GetType() == typeof(string)))
                    {
                        setVal = DateTime.Parse(value.ToString());
                    }
                    else if (_DataType == typeof(long) &&
                        (value.GetType() == typeof(int) || value.GetType() == typeof(long)))
                    {
                        setVal = (long)value;
                    }
                    else if (_DataType == typeof(string))
                    {
                        setVal = value.ToString();
                    }
                    else if (_DataType == typeof(object))
                    {
                        setVal = value;
                    }


                }
                if (setVal == null && _Nullable == false )
                {
                    throw new Exception($@"can not set {value.GetType().ToString()} to {_DataType.ToString()} ");
                }

                if (setVal != _Value)
                {
                    _Value = setVal;
                    _isChange = true;
                }
            }
        }
        public bool IsChange
        {
            get
            { return _isChange; }
        }

        public object OrgValue
        {
            get
            {
                return _OrgValue;
            }
            set
            {
                object setVal = null;
                if (value != null)
                {
                    if (_DataType == typeof(double) &&
                        (value.GetType() == typeof(int) || value.GetType() == typeof(double) || value.GetType() == typeof(float) ||
                          value.GetType() == typeof(int?) || value.GetType() == typeof(double?) || value.GetType() == typeof(float?)))
                    {
                        setVal = double.Parse(value.ToString());
                    }
                    else if (_DataType == typeof(DateTime) &&
                        (value.GetType() == typeof(DateTime)))
                    {
                        setVal = value;
                    }
                    else if (_DataType == typeof(DateTime) &&
                       (value.GetType() == typeof(string)))
                    {
                        setVal = DateTime.Parse(value.ToString());
                    }
                    else if (_DataType == typeof(long) &&
                        (value.GetType() == typeof(int) || value.GetType() == typeof(long)))
                    {
                        setVal = (long)value;
                    }
                    else if (_DataType == typeof(string))
                    {
                        if (value.ToString() == "")
                        {
                            setVal = null;
                        }
                        else
                        {
                            setVal = value.ToString();
                        }
                    }
                    else if (_DataType == typeof(object))
                    {
                        setVal = value;
                    }


                }
                if (setVal == null && _Nullable == false)
                {
                    throw new Exception($@"can not set {value.GetType().ToString()} to {_DataType.ToString()} ");
                }

               
               _OrgValue = setVal;
               _isChange = false;
                
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public Type DataType
        {
            get
            { return _DataType; }
        }

        public DataObjectItem(string name, object value, Type dataType,bool Nullable)
        {
            _Name = name;
            _Value = value;
            _OrgValue = value;
            _DataType = dataType;
            _isInit = true;
            _Nullable = Nullable;
        }
        public void Commit()
        {
            _isChange = false;
            _OrgValue = _Value;

        }

        public void Rollback()
        {
            _isChange = false;
            _Value = OrgValue;
        }
    }
}
