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
    public class CNoteAllController : Controller
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
        private I_Notes_Repository i_Notes_Repository;
        IMapper _mapper;
        public CNoteAllController([CustomerNoteAll] I_Notes_Repository i_Notes_Repository, IMapper _mapper)
        {
            this.i_Notes_Repository = i_Notes_Repository;
            this._mapper = _mapper;
        }

        [ChildActionOnly]
        public PartialViewResult NoteForm(string CardCode)
        {
            AddUpdateNoteViewModel addUpdateNoteViewModel = new AddUpdateNoteViewModel();
            addUpdateNoteViewModel.AjaxOptions = ajaxFormViewModel;
            addUpdateNoteViewModel.CardCode = CardCode;
            return PartialView("_AddUpdateNoteModal", addUpdateNoteViewModel);

        }
        [ChildActionOnly]
        public PartialViewResult NoteList(string CardCode)
        {
            IEnumerable<object> cNotesAll = i_Notes_Repository.GetNotesListByCardCode(CardCode);
            IEnumerable<NoteViewModel> CNotesAllList = _mapper.Map<IEnumerable<object>, IEnumerable<NoteViewModel>>(cNotesAll);
            NoteListViewModel ListModel = new NoteListViewModel() { NotesList = CNotesAllList, AjaxOptions = ajaxFormViewModel, CanEditNote= User.IsInRole("Notes")? true:false };

            return PartialView("_NotesList", ListModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AddNote(AddUpdateNoteViewModel model)
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

                    CNotesAll NoteObject = _mapper.Map<AddUpdateNoteViewModel, CNotesAll>(model);
                    i_Notes_Repository.SaveNotes(NoteObject);
                }
                else
                {
                    jsonResultViewModel.Opertation = OpertionType.UpdateRow;

                    CNotesAll NoteObject = (CNotesAll)i_Notes_Repository.GetNote(model.NoteID);
                    if (NoteObject != null)
                    {
                        NoteObject.Note = model.Note;
                        NoteObject.CardCode = model.CardCode;
                        NoteObject.UpdatedBy = User.Identity.Name;
                        NoteObject.UpdatedOn = DateTime.Now;
                        i_Notes_Repository.SaveNotes(NoteObject);
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
                IEnumerable<Object> cNotesAll = i_Notes_Repository.GetNotesListByCardCode(model.CardCode);
                IEnumerable<NoteViewModel> CNotesAllList = _mapper.Map<IEnumerable<Object>, IEnumerable<NoteViewModel>>(cNotesAll);

                NoteListViewModel ListModel = new NoteListViewModel() { NotesList = CNotesAllList, AjaxOptions = ajaxFormViewModel };
                jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_NotesList", ListModel));
            }

            jsonResultViewModel.Ax = ajaxFormViewModel;


            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditNote(int NoteID)
        {
            AddUpdateNoteViewModel EditNoteModel = new AddUpdateNoteViewModel();
            JsonResultViewModel jsonResultViewModel = new JsonResultViewModel() { };
            jsonResultViewModel.Opertation = OpertionType.EditRow;

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();

            if (ModelState.IsValid)
            {
                CNotesAll NoteObject = (CNotesAll)i_Notes_Repository.GetNote(NoteID);
                if (NoteObject != null)
                {
                    EditNoteModel = _mapper.Map<CNotesAll, AddUpdateNoteViewModel>(NoteObject);

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
            jsonResultViewModel.ContentToUpdateorReplace = PartialRender.RenderToString(PartialView("_AddUpdateNoteModal", EditNoteModel));
            jsonResultViewModel.Ax = ajaxFormViewModel;
            return Json(jsonResultViewModel, JsonRequestBehavior.DenyGet);
        }
    }
}