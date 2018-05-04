using System.Configuration;

namespace CafeNextFramework.CafeConfiguration
{
    public class ExcelConfiguration : ConfigurationElement
    {
        [ConfigurationProperty(Constant.EXCEL_SHEETNAME_ATTR)]
        public string Sheet
        {
            get
            {
                return (string)this[Constant.EXCEL_SHEETNAME_ATTR];
            }
        }

        [ConfigurationProperty(Constant.EXCEL_FILEPATH_ATTR)]
        public string FilePath
        {
            get
            {
                return (string)this[Constant.EXCEL_FILEPATH_ATTR];
            }
        }
    }
}