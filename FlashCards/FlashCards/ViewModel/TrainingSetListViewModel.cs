using System.Collections.Generic;

namespace FlashCards.ViewModel
{
    class TrainingSetListViewModel
    {
        public IEnumerable<TrainingSetListItemViewModel> Type { get; set; }
    }

    class TrainingSetListItemViewModel
    {
        public string Name { get; set; }
        public string QuestionsCount { get; set; }
    }
}
