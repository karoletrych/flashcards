using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FlashCards.Views;
using FlashCards.Views.Lesson;
using Xamarin.Forms;
using Xunit;
using AddLessonPage = FlashCards.Views.Lesson.AddLessonPage;

namespace FlashCards.UnitTests
{
    public class RegistrationsTests
    {
        [Fact]
        public void PagesTest()
        {
            var container = IocRegistrations.RegisterTypesInIocContainer();
            var pageFactory = container.Resolve<Func<string, NavigationPage>>();
            var page = pageFactory(nameof(AddLessonPage));

        }
    }
}
