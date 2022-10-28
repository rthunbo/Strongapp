using System.Globalization;
using CsvHelper;
using Strongapp.API.Controllers;
using Strongapp.API.Repositories;
using Strongapp.API.Services;
using Strongapp.Models;
using static Strongapp.Models.StrongExerciseBodyPart;
using static Strongapp.Models.StrongExerciseCategory;

namespace Strongapp.API.Database
{
    public class DbInitializer
    {
        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            IServiceProvider serviceProvider = applicationBuilder.ApplicationServices.CreateScope().ServiceProvider;
            
            var templateRepository = serviceProvider.GetRequiredService<TemplateRepository>();
            var workoutRepository = serviceProvider.GetRequiredService<WorkoutRepository>();
            var exerciseRepository = serviceProvider.GetRequiredService<ExerciseRepository>();
            var measurementRepository = serviceProvider.GetRequiredService<MeasurementRepository>();
            var workoutService = serviceProvider.GetRequiredService<WorkoutService>();

            var measuremments = await measurementRepository.GetAsync();
            if (!measuremments.Any())
            {
                var initialLoad = new List<StrongMeasurement>
                {
                    new StrongMeasurement { Name = "Weight", Date = DateTime.Parse("2018-02-18"), Value = 90 },
                    new StrongMeasurement { Name = "Weight", Date = DateTime.Parse("2018-06-06"), Value = 88 },
                    new StrongMeasurement { Name = "Weight", Date = DateTime.Parse("2019-09-08"), Value = 76 },
                    new StrongMeasurement { Name = "Weight", Date = DateTime.Parse("2020-10-20"), Value = 72 },
                    new StrongMeasurement { Name = "Weight", Date = DateTime.Parse("2020-11-25"), Value = 74 },
                    new StrongMeasurement { Name = "Weight", Date = DateTime.Parse("2021-05-14"), Value = 68 }
                };

                foreach (var e in initialLoad) await measurementRepository.CreateAsync(e);
            }

            var exercisesInitialLoad = GetExercises();
            var exercises = await exerciseRepository.GetAsync();
            if (!exercises.Any())
            {
                foreach (var e in exercisesInitialLoad) await exerciseRepository.CreateAsync(e);
            }

            var workouts = await workoutRepository.GetAsync();
            if (!workouts.Any())
            {
                using var reader = new StreamReader("strong.csv");
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                csv.Context.RegisterClassMap<CsvRecordMap>();
                var csvRecords = csv.GetRecords<CsvRecord>();

                var workoutsInitialLoad = csvRecords
                    .GroupBy(x => new { x.Date, x.WorkoutName, x.Duration })
                    .Select(g => new StrongWorkout
                    {
                        Date = g.Key.Date,
                        WorkoutName = g.Key.WorkoutName,
                        ExerciseData = g
                            .GroupBy(x => new { x.Date, x.WorkoutName, x.ExerciseName })
                            .Select(g => new StrongExerciseData
                            {
                                ExerciseName = g.Key.ExerciseName,
                                Sets = g.Select(x => new StrongExerciseSetData
                                {
                                    SetOrder = x.SetOrder,
                                    Distance = x.Distance,
                                    Reps = x.Reps,
                                    Seconds = x.Seconds,
                                    Weight = x.Weight
                                }).ToList()
                            }).ToList()
                    }).ToList();

                await workoutService.UpdatePersonalRecords(workoutsInitialLoad);
                foreach (var w in workoutsInitialLoad) await workoutRepository.CreateAsync(w);
            }

            var exampleTemplates = GetExampleTemplates();
            var templates = await templateRepository.GetAsync();
            if (!templates.Any())
            {
                foreach (var t in exampleTemplates) await templateRepository.CreateAsync(t);
            }
        }

        private static List<StrongTemplate> GetExampleTemplates()
        {
            var legsWorkout = new StrongTemplate
            {
                TemplateName = "Legs",
                IsExampleTemplate = true,
                ExerciseData = new List<StrongExerciseData>
                    {
                        new StrongExerciseData { ExerciseName = "Squat (Barbell)", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Leg Extension (Machine)", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Flat Leg Raise", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Standing Calf Raise (Dumbbell)", Sets = CreateSets(3, 10) },
                    }
            };

            var chestAndTricepsWorkout = new StrongTemplate
            {
                TemplateName = "Chest and Tricpes",
                IsExampleTemplate = true,
                ExerciseData = new List<StrongExerciseData>
                    {
                        new StrongExerciseData { ExerciseName = "Bench Press (Barbell)", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Incline Bench Press (Barbell)", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Strict Military Press (Barbell)", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Lateral Raise (Dumbbell)", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Skullcrusher (Barbell)", Sets = CreateSets(3, 10) },
                    }
            };

            var backAndBicepsWorkout = new StrongTemplate
            {
                TemplateName = "Back and Biceps",
                IsExampleTemplate = true,
                ExerciseData = new List<StrongExerciseData>
                    {
                        new StrongExerciseData { ExerciseName = "Deadlift (Barbell)", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Seated Row (Cable)", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Lat Pulldown (Cable)", Sets = CreateSets(3, 10) },
                        new StrongExerciseData { ExerciseName = "Bicep Curl (Barbell)", Sets = CreateSets(3, 10) },
                    }
            };

            var strong5x5WorkoutA = new StrongTemplate
            {
                TemplateName = "Strong 5x5 - Workout A",
                IsExampleTemplate = true,
                ExerciseData = new List<StrongExerciseData>
                    {
                        new StrongExerciseData { ExerciseName = "Squat (Barbell)", Sets = CreateSets(5, 5) },
                        new StrongExerciseData { ExerciseName = "Bench Press (Barbell)", Sets = CreateSets(5, 5) },
                        new StrongExerciseData { ExerciseName = "Bent Over Row (Barbell)", Sets = CreateSets(5, 5) }
                    }
            };

            var strong5x5WorkoutB = new StrongTemplate
            {
                TemplateName = "Strong 5x5 - Workout B",
                IsExampleTemplate = true,
                ExerciseData = new List<StrongExerciseData>
                    {
                        new StrongExerciseData { ExerciseName = "Squat (Barbell)", Sets = CreateSets(5, 5) },
                        new StrongExerciseData { ExerciseName = "Overhead Press (Barbell)", Sets = CreateSets(5, 5) },
                        new StrongExerciseData { ExerciseName = "Deadlift (Barbell)", Sets = CreateSets(5, 5) }
                    }
            };

            var exampleTemplates = new List<StrongTemplate>
                {
                    legsWorkout,
                    chestAndTricepsWorkout,
                    backAndBicepsWorkout,
                    strong5x5WorkoutA,
                    strong5x5WorkoutB
                };
            return exampleTemplates;
        }

        private static List<StrongExercise> GetExercises()
        {
            return new()
            {
                new StrongExercise { ExerciseName = "Running", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.Cardio},
                new StrongExercise { ExerciseName = "Arche Up Hold", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Duration},
                new StrongExercise { ExerciseName = "Arche Up Reps", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Arnold Press (Dumbbell)", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.Dumbbell },
                new StrongExercise { ExerciseName = "Bat Wings", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Bear Hug Carry (Plates)", BodyPart  = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Bench Dip", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Bench Press (Barbell)", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Bench Press (Dumbbell)", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Bent Over Raises (Dumbbell)" , BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Bent Over Row", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Bent Over Row (Band)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Bent Over Row (Barbell)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Bent Over Row (Dumbbell)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Bicep Curl (Barbell)", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Bicep Curl (Cable)", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Body Rows", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Bulgarian Split Squat", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Cable Crossover", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Cable Fly", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Cable Lateral Raise", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Cable Twist", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Calves on Leg Press", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Chest Dip", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Chest Fly", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Chest Fly (Dumbbell)", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Chest Press", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Chest Press (Machine)", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Chest Supported Rows (Dumbbell)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Chin Up", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Chin Up (Assisted)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.AssistedBodyweight},
                new StrongExercise { ExerciseName = "Chin Up (Neutral Grip)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Close Grip Bench Press", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Crunch", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Crunch (Machine)", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Deadlift (Barbell)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Deadlift High Pull (Barbell)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Deficit Deadlift", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Deltoid Fly", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Dips", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Dunbbell Fly", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Face Pull", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Face Pull (Cable)", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Farmers Walk", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Farmers Walk (Kettlebell)", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Forearm Bicep Curl", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Front Loaded Carry", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Front Squat", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Front Squat (Barbell)", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Glute Bridge", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Goblet Squat (Kettlebell)", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Hammer Curl (Dumbbell)", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Hanging Knee Raise", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Hanging Leg Raise", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Hip Thrust (Bodyweight)", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Incline Bench Press (Barbell)", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Incline Bench Press (Dumbbell)", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Inverse Bicep Curl", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Inverted Row (Bodyweight)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Lat Pulldown - Underhand (Cable)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Lat Pulldown (Cable)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Lat Pulldown (Close Grip)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Lat Pulldown (Single Arm)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Lateral Raise (Dumbbell)", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Leg Curl", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Lu Raises", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Lunge (Bodyweight)", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Medium Grip Bench Press", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Negative Chin Up", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Negative Pull Up", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Oblique Crunch", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Olympic Squat", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Overhead Carry", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Overhead Press (Dumbbell)", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Overhead Squat", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Paused Deadlift", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Paused Squat", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Plank", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.Duration},
                new StrongExercise { ExerciseName = "Plank Reps", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Power Clean", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Pull Down (Narrow Grip)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Pull Down (Wide Grip)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Pull Up", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.WeightedBodyweight },
                new StrongExercise { ExerciseName = "Pull Up (Assisted)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.AssistedBodyweight },
                new StrongExercise { ExerciseName = "Push Up", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Push Up (Close Grip)", BodyPart = StrongExerciseBodyPart.Chest, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Reverse Curl (Barbell)", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Romanian Deadlift", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Romanian Deadlift (Barbell)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Seal Row", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Seated Calf-Raise", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Seated Row", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Seated Row (Cable)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Seated Row (Wide Grip)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Shoulder Press (Dumbbell)", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Shoulder Press (Machine)", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Shrug (Dumbbell)", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Side Plank", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.Duration},
                new StrongExercise { ExerciseName = "Snatch Grip Deadlift", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Squat (Barbell)", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Squat (Bodyweight)", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.WeightedBodyweight},
                new StrongExercise { ExerciseName = "Squat (Smith Machine)", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Standing Calf Raise (Dumbbell)", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Standing Calf Raise (Machine)", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Step Up (Dumbbell)", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Step-up", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Stiff Leg Deadlift (Barbell)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Strict Military Press (Barbell)", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Superman", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "Supporting Knee Raise", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.RepsOnly},
                new StrongExercise { ExerciseName = "T-Bar Row", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "T-Bar Row (Wide Grip)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Tempo Squat", BodyPart = StrongExerciseBodyPart.Legs, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Torso Rotation (Machine)", BodyPart = StrongExerciseBodyPart.Core, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Tricep Extension (Cable)", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Tricep Pushdown", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Triceps Extension", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Triceps Extension (Dumbbell)", BodyPart = StrongExerciseBodyPart.Arms, Category = StrongExerciseCategory.Dumbbell},
                new StrongExercise { ExerciseName = "Upper Back", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.MachineOther},
                new StrongExercise { ExerciseName = "Upright Row (Barbell)", BodyPart = StrongExerciseBodyPart.Back, Category = StrongExerciseCategory.Barbell},
                new StrongExercise { ExerciseName = "Viking Press", BodyPart = StrongExerciseBodyPart.Shoulders, Category = StrongExerciseCategory.MachineOther}
            };
        }

        private static List<StrongExerciseSetData> CreateSets(int setCount, int repCount)
        {
            List<StrongExerciseSetData> sets = new();
            for (int setIndex = 1; setIndex <= setCount; setIndex++)
            {
                sets.Add(new StrongExerciseSetData { SetOrder = setIndex, Reps = repCount });
            }
            return sets;
        }
    }
}
