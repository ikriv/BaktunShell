/* Copyright 2009 Ivan Krivyakov

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using System.Windows;

namespace Ikriv.Surfaces
{
    public class PropertyHolder<PropertyType, HoldingType> where HoldingType:DependencyObject
    {
        DependencyProperty _property;

        public PropertyHolder(string name, PropertyType defaultValue, PropertyChangedCallback propertyChangedCallback)
        {
            _property = 
                DependencyProperty.Register(
                    name, 
                    typeof(PropertyType), 
                    typeof(HoldingType), 
                    new PropertyMetadata(defaultValue, propertyChangedCallback));
        }

        public DependencyProperty Property
        {
            get { return _property; }
        }

        public PropertyType Get(HoldingType obj)
        {
            return (PropertyType)obj.GetValue(_property);
        }

        public void Set(HoldingType obj, PropertyType value)
        {
            obj.SetValue(_property, value);
        }
    }
}
