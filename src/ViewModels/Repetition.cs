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
    public class Repetition : IRepetition
    {
        private readonly ISpacedRepetition _spacedRepetition;
        private readonly ExaminerBuilder _examinerBuilder;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly ISetting<int> _maximumFlashcardsInRepetitionSetting;
        private readonly ISetting<AskingMode> _repetitionAskingModeSetting;
        
        
        public Repetition(ISpacedRepetition spacedRepetition,
            ExaminerBuilder examinerBuilder,
            IRepository<Lesson> lessonRepository,
            ISetting<int> maximumFlashcardsInRepetitionSetting,
            ISetting<AskingMode> repetitionAskingModeSetting
	        )
        {
            _spacedRepetition = spacedRepetition;
            _examinerBuilder = examinerBuilder;
            _lessonRepository = lessonRepository;
            _maximumFlashcardsInRepetitionSetting = maximumFlashcardsInRepetitionSetting;
            _repetitionAskingModeSetting = repetitionAskingModeSetting;
        }

        public async Task Repeat(INavigationService navigationService)
        {
            var repetitionFlashcards = await _spacedRepetition.CurrentRepetitionFlashcards();
            var activeLessons = await _lessonRepository
                .Where(lesson => lesson.AskInRepetitions);
            var activeFlashcards = activeLessons.SelectMany(lesson => lesson.Flashcards);
            
            var flashcardsToAsk = repetitionFlashcards
                .Intersect(activeFlashcards)
                .Take(_maximumFlashcardsInRepetitionSetting.Value)
                .ToList();

            if (!flashcardsToAsk.Any())
                throw new InvalidOperationException("No flashcards to ask");
            
            var examiner = _examinerBuilder
                .WithFlashcards(flashcardsToAsk)
                .WithRepeatingQuestions(true)
                .WithAskingMode(_repetitionAskingModeSetting.Value)
                .Build();

            await navigationService.NavigateAsync("NavigationPage/LessonListPage/AskingQuestionsPage",
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