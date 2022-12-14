using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.WebUI.Helpers.HtmlHelpers;
using BMSS.WebUI.Infrastructure;
using BMSS.WebUI.Models.DocNumberingViewModels;
using BMSS.WebUI.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    public class NumberingPOController : Controller
    {
        private AjaxFormViewModel ajaxFormViewModel = new AjaxFormViewModel()
        {
            FormAddAction = "AddNumberingPO",
            AddActionTargetId = "NumberingPOListTble",
            AddFormId = "NumberingPOAddForm",
            FormDeleteAction = "DeleteNumberingPO",
            DeleteActionTargetId = "NumberingPOListTble",
            DeleteFormId = "NumberingPODeleteForm",
            FormEditAction = "EditNumberingPO",
            EditActionTargetId = "NumberingPOListTble",
            EditFormId = "NumberingPOEditForm",
            FormUpdateAction = "AddNumberingPO",
            UpdateActionTargetId = "NumberingPOListTble",
            UpdateFormId = "NumberingPOAddForm",
            ModalID = "AddUpdateNumberingPOModal"
        };
        //Repositories
        private I_Numbering_Repository i_Numbering_Repository;
        IMapper _mapper;

        public NumberingPOController([NumPO] I_Numbering_Repository i_Numbering_Repository, IMapper _mapper)
        {
            this.i_Numbering_Repository = i_Numbering_Repository;
            this._mapper = _mapper;
        }
        [ChildActionOnly]
        public PartialViewResult NumberingForm()
        {
            AddUpdateDocNumberingViewModel addUpdateDocNumberingViewModel = new AddUpdateDocNumberingViewModel();
            addUpdateDocNumberingViewModel.AjaxOptions = ajaxFormViewModel;
            return PartialView("_AddUpdateDocNumberingModal", addUpdateDocNumberingViewModel);

        }
        [ChildActionOnly]
        public PartialViewResult NumberingList(string CardCode)
        {
            IEnumerable<object> NumberingObjList = i_Numbering_Repository.NumberingList;
            IEnumerable<DocNumberingViewModel> NumberingObjViewList = _mapper.Map<IEnumerable<object>, IEnumerable<DocNumberingViewModel>>(NumberingObjList);
            DocNumberingListViewModel ListModel = new DocNumberingListViewModel() { NumberingList = NumberingObjViewList, AjaxOptions = ajaxFormViewModel };

            return PartialView("_NumberingList", ListModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AddNumberingPO(AddUpdateDocNumberingViewModel model)
        {
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };

            jsonResultViewModel.Opertation = OpertionType.AddRow;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {

                if (model.NumberingID == 0)
                {

                    if (model.IsDefault == false && i_Numbering_Repository.NumberingList.Count() == 0)
                    {
                        model.IsDefault = true;
                    }
                    if (i_Numbering_Repository.IsSeriesNameAlreadyExist(model.SeriesName))
                    {
                        ErrList.Add("Series Name already exist");
                    }
                    else if (model.FirstNo >= model.LastNo)
                    {
                        ErrList.Add("First No cannot be greater than Last No");
                    }
                    else if (i_Numbering_Repository.IsSeriesOverlaps(model.FirstNo, model.LastNo))
                    {
                        ErrList.Add("Series overlaps with existing Serieses");
                    }

                    else
                    {
                        NumberingPO NumberingObject = _mapper.Map<AddUpdateDocNumberingViewModel, NumberingPO>(model);
                        NumberingObject.CreatedBy = User.Identity.Name;
                        i_Numbering_Repository.SaveNumberings(NumberingObject);
                    }

                }
                else
                {
                    jsonResultViewModel.Opertation = OpertionType.UpdateRow;
                    NumberingPO NumberingObject = (NumberingPO)i_Numbering_Repository.GetNumbering(model.NumberingID);
                    if (NumberingObject != null)
                    {
                        if (NumberingObject.IsLocked.Equals(false))
                        {
                            if (i_Numbering_Repository.IsSeriesNameAlreadyExist(model.SeriesName, model.NumberingID))
                            {
                                ErrList.Add("Series Name already exist");
                            }
                            else if (model.FirstNo >= model.LastNo)
                            {
                                ErrList.Add("First No cannot be greater than Last No");
                            }
                            else if (i_Numbering_Repository.IsSeriesOverlaps(model.FirstNo, model.LastNo, model.NumberingID))
                            {
                                ErrList.Add("Series overlaps with existing Serieses");
                            }
                            else
                            {

                                NumberingObject = _mapper.Map<AddUpdateDocNumberingViewModel, NumberingPO>(model);

                                NumberingObject.UpdatedBy = User.Identity.Name;

                                i_Numbering_Repository.SaveNumberings(NumberingObject);

                            }
                        }
                        else
                        {
                            ErrList.Add("Series already locked, cannot proceed");
                        }
                    }
                    else
                    {
                        ErrList.Add("Numbering Series not exist");
                    }
                }
            }
            else
            {
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
            }

            if (ModelErrList.Count() > 0 || ErrList.Count > 0)
            {
                jsonResultViewModel.IsOpertationSuccess = false;
                foreach (var entry in ModelErrList)
                {
                    ErrList.Add(entry);
                }
                jsonResultViewModel.ErrList = ErrList;
                jsonResultViewModel.IsOpertationSuccess = false;
            }

            if (jsonResultViewModel.IsOpertationSuccess)
            {
                IEnumerable<Object> NumberingList = i_Numbering_Repository.NumberingList;
                IEnumerable<DocNumberingViewModel> DocNumberingViewModelList = _mapper.Map<IEnumerable<Object>, IEnumerable<DocNumberingViewModel>>(NumberingList);

                DocNumberingListViewModel ListModel = new DocNumberingListViewModel() { NumberingList = DocNumberingViewModelList, AjaxOptions = ajaxFormViewModel };
                jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_NumberingList", ListModel));
            }

            jsonResultViewModel.Ax = ajaxFormViewModel;


            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditNumberingPO(int NumberingID)
        {
            AddUpdateDocNumberingViewModel EditNumberingModel = new AddUpdateDocNumberingViewModel();
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };
            jsonResultViewModel.Opertation = OpertionType.EditRow;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {
                NumberingPO NumberingObject = (NumberingPO)i_Numbering_Repository.GetNumbering(NumberingID);
                if (NumberingObject != null)
                {
                    if (NumberingObject.IsLocked.Equals(false))
                    {

                        EditNumberingModel = _mapper.Map<NumberingPO, AddUpdateDocNumberingViewModel>(NumberingObject);
                    }
                    else
                    {
                        ErrList.Add("Series already locked, cannot proceed");
                    }
                }
                else
                {
                    ErrList.Add("Numbering Series not exist");
                }

            }
            else
            {
                ErrList.Add("Note not exist");
            }
            if (ModelErrList.Count() > 0 || ErrList.Count > 0)
            {
                jsonResultViewModel.IsOpertationSuccess = false;
                foreach (var entry in ModelErrList)
                {
                    ErrList.Add(entry);
                }
                jsonResultViewModel.ErrList = ErrList;
                jsonResultViewModel.IsOpertationSuccess = false;
            }
            EditNumberingModel.AjaxOptions = ajaxFormViewModel;
            jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_AddUpdateDocNumberingModal", EditNumberingModel));
            jsonResultViewModel.Ax = ajaxFormViewModel;
            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult DeleteNumberingPO(int NumberingID)
        {
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };

            jsonResultViewModel.Opertation = OpertionType.DeleteRow;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {
                NumberingPO NumberingObject = (NumberingPO)i_Numbering_Repository.GetNumbering(NumberingID);




                if (NumberingObject != null)
                {
                    if (NumberingObject.IsLocked.Equals(false))
                    {
                        if (i_Numbering_Repository.IsDefault(NumberingID))
                        {
                            ErrList.Add("Cannot delete default Series");
                        }
                        else
                        {
                            i_Numbering_Repository.Remove(NumberingID);
                        }
                    }
                    else
                    {
                        ErrList.Add("Series already locked, cannot proceed");
                    }
                }
                else
                {
                    ErrList.Add("Numbering Series not exist");
                }

            }
            else
            {
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
            }

            if (ModelErrList.Count() > 0 || ErrList.Count > 0)
            {
                jsonResultViewModel.IsOpertationSuccess = false;
                foreach (var entry in ModelErrList)
                {
                    ErrList.Add(entry);
                }
                jsonResultViewModel.ErrList = ErrList;
                jsonResultViewModel.IsOpertationSuccess = false;
            }

            if (jsonResultViewModel.IsOpertationSuccess)
            {
                IEnumerable<Object> NumberingList = i_Numbering_Repository.NumberingList;
                IEnumerable<DocNumberingViewModel> DocNumberingViewModelList = _mapper.Map<IEnumerable<Object>, IEnumerable<DocNumberingViewModel>>(NumberingList);

                DocNumberingListViewModel ListModel = new DocNumberingListViewModel() { NumberingList = DocNumberingViewModelList, AjaxOptions = ajaxFormViewModel };
                jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_NumberingList", ListModel));
            }

            jsonResultViewModel.Ax = ajaxFormViewModel;


            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }
    }
}