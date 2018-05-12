using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemporaryEmploymentCorp.Helpers.ReportViewer
{
    public class DataSetValuePair
    {
        public string DataSetName { get; private set; }
        public object DataSource { get; private set; }

        public DataSetValuePair(string dataSetName, object dataSource)
        {
            DataSetName = dataSetName;
            DataSource = dataSource;
        }
    }
}
