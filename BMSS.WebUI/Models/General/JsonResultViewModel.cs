using System.Collections.Generic;

namespace BMSS.WebUI.Models.General
{
    public class JsonResultViewModel
    {
        private bool isOpertationSuccess = true;
          
        public bool IsOpertationSuccess
        {
            get
            {
                return isOpertationSuccess;
            }
            set
            {
                isOpertationSuccess = value;
            }

        }
        private bool isResultDataTableOpertation = false;
        public bool IsResultDataTableOpertation
        {
            get
            {
                return isResultDataTableOpertation;
            }
            set
            {
                isResultDataTableOpertation = value;
            }

        }      
        public OpertionType Opertation { get; set; }     
        public List<string> rowValues { get; set; }

        public string ContentToUpdateorReplace { get; set; }
        public string ContentToLoadNewForm { get; set; }
        public List<string> ErrList { get; set; }
        public AjaxFormViewModel Ax { get; set; }
    }
}