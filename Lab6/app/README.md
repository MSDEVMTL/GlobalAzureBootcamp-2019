# Steps (needs cleaning and better scenario)
1. ARM template to add function app
   1. Add explanation here for what to add to the template
2. in ADO create new build pipeline using yaml and target build.yaml, it should trigger on every commit (CI)
   1. yaml should target the csproj of the app (may need adjustment)
3. in ADO create relase pipeline using generated artifact from yaml, it should trigger when build is done (CD)
4. in DogImage.cs
   1. Fill out connection string
   2. Add code to call VisionAPI and check if it's a dog
   3. `code here`
5. Commit code to repo... should trigger a build and then a release
6. Use provided assets to add all images in the container (storage explorer)
7. Function should trigger... check what is left of assets that were dogs
8. *Extra credit*: move non-dog assets to another container using the available function overload (output blob or something... must check... just like webjobs)