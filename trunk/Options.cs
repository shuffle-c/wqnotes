/*
* Copyright (c) 2007-2008 wqNotes Project
* License: BSD
* Windows: Options.cs, revision $Revision$
* URL: $HeadURL$
* $Date$
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace wqNotes
{
   #region Сортировка
   public class PropertySorter : ExpandableObjectConverter
   {
      public override bool GetPropertiesSupported(ITypeDescriptorContext context)
      {
         return true;
      }

      /// <summary>
      /// Возвращает упорядоченный список свойств
      /// </summary>
      public override PropertyDescriptorCollection GetProperties(
         ITypeDescriptorContext context, object value, Attribute[] attributes)
      {
         PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
         ArrayList orderedProperties = new ArrayList();
         foreach (PropertyDescriptor pd in pdc)
         {
            Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
            if (attribute != null)
            {
               // атрибут есть - используем номер п/п из него
               PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
               orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
            }
            else
            {
               // атрибута нет - считаем что 0
               orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
            }
         }

         // сортируем по Order-у
         orderedProperties.Sort();

         // формируем список имен свойств
         ArrayList propertyNames = new ArrayList();
         foreach (PropertyOrderPair pop in orderedProperties)
         {
            propertyNames.Add(pop.Name);
         }

         // возвращаем
         return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
      }
   }

   /// <summary>
   /// Атрибут для задания сортировки
   /// </summary>
   [AttributeUsage(AttributeTargets.Property)]
   public class PropertyOrderAttribute : Attribute
   {
      private int _order;
      public PropertyOrderAttribute(int order)
      {
         _order = order;
      }

      public int Order
      {
         get { return _order; }
      }
   }

   /// <summary>
   /// Пара имя/номер п/п с сортировкой по номеру
   /// </summary>
   public class PropertyOrderPair : IComparable
   {
      private int _order;
      private string _name;
      public string Name
      {
         get { return _name; }
      }

      public PropertyOrderPair(string name, int order)
      {
         _order = order;
         _name = name;
      }

      /// <summary>
      /// Собственно метод сравнения
      /// </summary>
      public int CompareTo(object obj)
      {
         int otherOrder = ((PropertyOrderPair)obj)._order;
         if (otherOrder == _order)
         {
            // если Order одинаковый - сортируем по именам
            string otherName = ((PropertyOrderPair)obj)._name;
            return string.Compare(_name, otherName);
         }
         else if (otherOrder > _order)
         {
            return -1;
         }
         return 1;
      }
   }
   #endregion

   #region Преобразования boolean
   interface IBooleanString
   {
      string True { get; }
      string False { get; }
   }

   class BooleanYesNo : IBooleanString
   {
      public string True
      {
         get { return "Да"; }
      }
      public string False
      {
         get { return "Нет"; }
      }
   }

   class BooleanTypeConverter<T> : BooleanConverter where T : IBooleanString, new()
   {
      private T bclass = new T();
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
      {
         return (bool)value ? bclass.True : bclass.False;
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
      {
         return (string)value == bclass.True;
      }
   }
   #endregion

   [Serializable]
   [TypeConverter(typeof(PropertySorter))]
   public class Options
   {
      public Options() { }

      public Options Clone()
      {
         IFormatter serializer = new BinaryFormatter();
         MemoryStream stream = new MemoryStream();
         serializer.Serialize(stream, this);
         stream.Seek(0, SeekOrigin.Begin);
         return (Options)serializer.Deserialize(stream);
      }

      static public Options Load()
      {
         if (Properties.Settings.Default.GeneralOptions == "")
            return new Options();

         Options opt;
         try
         {
            IFormatter serializer = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(
               Convert.FromBase64String(
               Properties.Settings.Default.GeneralOptions));
            opt = (Options)serializer.Deserialize(stream);
         }
         catch { opt = new Options(); }
         return opt;
      }

      public void Save()
      {
         IFormatter serializer = new BinaryFormatter();
         MemoryStream stream = new MemoryStream();
         serializer.Serialize(stream, this);
         stream.Seek(0, SeekOrigin.Begin);
         Properties.Settings.Default.GeneralOptions =
            Convert.ToBase64String(stream.ToArray());
         //Properties.Settings.Default.Save();
      }

      private bool _IsHideOnMinimize = false;
      [DisplayName("Скрывать при сворачивании")]
      [Description("Скрывать окно программы с панели задач при сворачивании")]
      [Category("1. Основные")]
      [TypeConverter(typeof(BooleanTypeConverter<BooleanYesNo>))]
      public bool IsHideOnMinimize
      {
         get { return _IsHideOnMinimize; }
         set { _IsHideOnMinimize = value; }
      }

      private bool _LoadLastFile = false;
      [DisplayName("Загружать последний файл")]
      [Description("Загружать последний открытый файл при загрузке программы")]
      [Category("1. Основные")]
      [TypeConverter(typeof(BooleanTypeConverter<BooleanYesNo>))]
      public bool LoadLastFile
      {
         get { return _LoadLastFile; }
         set { _LoadLastFile = value; }
      }

      private Int32 _CountRecentFiles = 4;
      [DisplayName("Количество недавних файлов")]
      [Description("Указывает, сколько последних открытых файлов будет отображаться в меню Файл")]
      [Category("1. Основные")]
      public Int32 CountRecentFiles
      {
         get { return _CountRecentFiles; }
         set { if (value >= 0) _CountRecentFiles = value; }
      }

      private Font _FontRichEdit = new Font("Lucida Console", 8.25F, FontStyle.Regular);
      [DisplayName("Шрифт окна заметок")]
      [Description("Шрифт, используемый по умолчанию для текста заметок")]
      [Category("2. Оформление")]
      public Font FontRichEdit
      {
         get { return _FontRichEdit; }
         set { _FontRichEdit = value; }
      }
   }
}
