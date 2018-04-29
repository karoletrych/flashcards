# Flashcards

Flashcards is a language learning mobile app based on spaced repetition techniques created with .NET and Xamarin.Forms.

### Features
  - Automatic translation when adding new flashcards
  - Notifications reminding about repetitions
  - Export and import flashcards database

<table>
	<tr>
		<td> <img src="https://i.imgur.com/R2uMcWf.png" height="300;"/> </td>
		<td> <img src="https://i.imgur.com/E1CAnTq.png" height="300;"/> </td>
		<td> <img src="https://i.imgur.com/FMaFsME.png" height="300;"/> </td>
	</tr>
</table>

### Development
 - Created with Xamarin.Forms.
 - Follows MVVM pattern and Onion Architecture.
 - New spaced repetition algorithms can be added by implementing interfaces from `Flashcards.Domain.SpacedRepetition.Interface` namespace.

### Todos
 - [ ] importing flashcards from camera images using OCR (Tesseract-OCR)
 - [ ] more spaced repetition algorithms based on learning curve or neural networks (TensorFlow)
 - [ ] make scheduling alarms platform independent
 - [ ] iOS version

### Google Play
https://play.google.com/store/apps/details?id=pl.karoletrych.Flashcards

License
----
MIT


