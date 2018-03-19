using System;
using System.Linq;
using System.Threading.Tasks;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.DataAccess;
using Flashcards.Settings;
using Flashcards.SpacedRepetition.Interface;
using Prism.Navigation;

namespace Flashcards.ViewModels
{
    public class Repetition
    {
        private readonly ISpacedRepetition _spacedRepetition;
        private readonly INavigationService _navigationService;
        private readonly ExaminerBuilder _examinerBuilder;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly ISetting<int> _maximumFlashcardsInLessonSetting;
        private readonly ISetting<AskingMode> _repetitionAskingMode;
        
        
        public Repetition(ISpacedRepetition spacedRepetition,
            INavigationService navigationService,
            ExaminerBuilder examinerBuilder,
            IRepository<Lesson> lessonRepository,
            ISetting<int> maximumFlashcardsInLessonSetting,
            ISetting<AskingMode> repetitionAskingMode)
        {
            _spacedRepetition = spacedRepetition;
            _navigationService = navigationService;
            _examinerBuilder = examinerBuilder;
            _lessonRepository = lessonRepository;
            _maximumFlashcardsInLessonSetting = maximumFlashcardsInLessonSetting;
            _repetitionAskingMode = repetitionAskingMode;
        }

        public async Task Repeat()
        {
            var repetitionFlashcards = await _spacedRepetition.CurrentRepetitionFlashcards();
            var activeLessons = await _lessonRepository
                .Where(lesson => lesson.AskInRepetitions);
            var activeFlashcards = activeLessons.SelectMany(lesson => lesson.Flashcards);
            
            var flashcardsToAsk = repetitionFlashcards
                .Intersect(activeFlashcards)
                .Take(_maximumFlashcardsInLessonSetting.Value)
                .ToList();

            if (!flashcardsToAsk.Any())
                throw new InvalidOperationException("No flashcards to ask");
            
            var examiner = _examinerBuilder
                .WithFlashcards(flashcardsToAsk)
                .WithRepeatingQuestions(true)
                .WithAskingMode(_repetitionAskingMode.Value)
                .Build();

            await _navigationService.NavigateAsync("NavigationPage/LessonListPage/AskingQuestionsPage",
                new NavigationParameters
                {
                    {
                        "examiner",
                        examiner
                    }
                });
            var results = await examiner.QuestionResults.Task;

            _spacedRepetition.RearrangeFlashcards(results);
        }
    }
}