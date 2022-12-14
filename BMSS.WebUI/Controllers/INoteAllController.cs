using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.WebUI.Helpers.HtmlHelpers;
using BMSS.WebUI.Infrastructure;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.NotesViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class INoteAllController : Controller
    {
        private AjaxFormViewModel ajaxFormViewModel = new AjaxFormViewModel()
        {
            FormAddAction = "AddNote",
            AddActionTargetId = "NoteListTble",
            AddFormId = "NoteAddForm",
            FormDeleteAction = "",
            DeleteActionTargetId = "",
            DeleteFormId = "",
            FormEditAction = "EditNote",
            EditActionTargetId = "NoteListTble",
            EditFormId = "NoteEditForm",
            FormUpdateAction = "AddNote",
            UpdateActionTargetId = "NoteListTble",
            UpdateFormId = "NoteAddForm",
            ModalID = "AddUpdateNoteModal"
        };
        //Repositories
        private I_ItmNotes_Repository i_ItmNotes_Repository;
        IMapper _mapper;
        public INoteAllController([ItemNoteAll] I_ItmNotes_Repository i_ItmNotes_Repository, IMapper _mapper)
        {
            this.i_ItmNotes_Repository = i_ItmNotes_Repository;
            this._mapper = _mapper;
        }
        [ChildActionOnly]
        public PartialViewResult NoteForm(string ItemCode)
        {
            AddUpdateItemNoteViewModel addUpdateItemNoteViewModel = new AddUpdateItemNoteViewModel();
            addUpdateItemNoteViewModel.AjaxOptions = ajaxFormViewModel;
            addUpdateItemNoteViewModel.ItemCode = ItemCode;
            return PartialView("_AddUpdateItemNoteModal", addUpdateItemNoteViewModel);

        }
        [ChildActionOnly]
        public PartialViewResult NoteList(string ItemCode)
        {
            IEnumerable<object> INotesAll = i_ItmNotes_Repository.GetNotesListByItemCode(ItemCode);
            IEnumerable<ItemNoteViewModel> INotesAllList = _mapper.Map<IEnumerable<object>, IEnumerable<ItemNoteViewModel>>(INotesAll);
            ItemNoteListViewModel ListModel = new ItemNoteListViewModel() { NotesList = INotesAllList, AjaxOptions = ajaxFormViewModel };
            return PartialView("_ItemNotesList", ListModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AddNote(AddUpdateItemNoteViewModel model)
        {
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };

            jsonResultViewModel.Opertation = OpertionType.AddRow;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {

                if (model.NoteID == 0)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedOn = DateTime.Now;

                    INotesAll NoteObject = _mapper.Map<AddUpdateItemNoteViewModel, INotesAll>(model);
                    i_ItmNotes_Repository.SaveNotes(NoteObject);
                }
                else
                {
                    jsonResultViewModel.Opertation = OpertionType.UpdateRow;

                    INotesAll NoteObject = (INotesAll)i_ItmNotes_Repository.GetNote(model.NoteID);
                    if (NoteObject != null)
                    {
                        NoteObject.Note = model.Note;
                        NoteObject.ItemCode = model.ItemCode;
                        NoteObject.UpdatedBy = User.Identity.Name;
                        NoteObject.UpdatedOn = DateTime.Now;
                        i_ItmNotes_Repository.SaveNotes(NoteObject);
                    }
                    else
                    {
                        ErrList.Add("Note not exist");
                    }

                }
            }
            else
            {
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
            }

            if (ModelErrList.Count() > 0 || ErrList.Count > 0)
            {

                foreach (var entry in ModelErrList)
                {
                    ErrList.Add(entry);
                }
                jsonResultViewModel.ErrList = ErrList;
                jsonResultViewModel.IsOpertationSuccess = false;
            }

            if (jsonResultViewModel.IsOpertationSuccess)
            {
                IEnumerable<Object> iNotesAll = i_ItmNotes_Repository.GetNotesListByItemCode(model.ItemCode);
                IEnumerable<ItemNoteViewModel> INotesAllList = _mapper.Map<IEnumerable<Object>, IEnumerable<ItemNoteViewModel>>(iNotesAll);

                ItemNoteListViewModel ListModel = new ItemNoteListViewModel() { NotesList = INotesAllList, AjaxOptions = ajaxFormViewModel };
                jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_ItemNotesList", ListModel));
            }

            jsonResultViewModel.Ax = ajaxFormViewModel;


            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditNote(int NoteID)
        {
            AddUpdateItemNoteViewModel EditNoteModel = new AddUpdateItemNoteViewModel();
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };
            jsonResultViewModel.Opertation = OpertionType.EditRow;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {
                INotesAll NoteObject = (INotesAll)i_ItmNotes_Repository.GetNote(NoteID);
                if (NoteObject != null)
                {
                    EditNoteModel = _mapper.Map<INotesAll, AddUpdateItemNoteViewModel>(NoteObject);

                }
                else
                {
                    ErrList.Add("Note not exist");
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
            EditNoteModel.AjaxOptions = ajaxFormViewModel;
            jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_AddUpdateItemNoteModal", EditNoteModel));
            jsonResultViewModel.Ax = ajaxFormViewModel;
            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }
    }
}