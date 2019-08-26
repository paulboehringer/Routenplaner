using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using Routenplaner.ViewModels;
using Services.Core;
using Services.Directions;
using Services.GeoCoding;
using Services.StaticMaps;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RoutenPlanerUnitTests.ViewModels
{
    /// <summary>
    /// Test class for RoutenVieModel
    /// </summary>
    [TestClass]
    public class RoutenViewModelTests
    {
        #region Constants and private vars
        const string origin = "Breiteweg 17, 30, 06 Bern";
        const string via = "Ziegelfeldstrasse, 4600 Olten";
        const string destination = "Zürich Flughafen, 8302 Kloten";
        const string retOrigin = "Breiteweg 17, 30, 06 Bern, Schweiz";
        const string retVia = "Ziegelfeldstrasse, 4600 Olten, Schweiz";
        const string retDstination = "Zürich Flughafen, 8302 Kloten, Schweiz";

        private RoutenViewModel viewModel;
        private AutoMocker autoMocker;
        Mock<IGeoCoding> geoCodingMock;
        Mock<IStaticMaps> staticMapsMock;
        Mock<IDirections> directionsMock;
        #endregion Constants and private vars

        #region Initialize and Cleanup
        [TestInitialize]
        public void Initialize()
        {
            autoMocker = new AutoMocker();
            geoCodingMock = autoMocker.GetMock<IGeoCoding>();
            staticMapsMock = autoMocker.GetMock<IStaticMaps>();
            directionsMock = autoMocker.GetMock<IDirections>();

            viewModel = new RoutenViewModel(geoCodingMock.Object, staticMapsMock.Object, directionsMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            viewModel.Dispose();
            viewModel = null;
            autoMocker = null;
        }
        #endregion Initialize and Cleanup

        #region TestMethods
        [TestMethod]
        public void ClearFieldsTest()
        {
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.RoutenModel.Origin));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.RoutenModel.Waypoint));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.RoutenModel.Destination));

            viewModel.ClearFieldsCommand.Execute();

            Assert.IsTrue(string.IsNullOrEmpty(viewModel.RoutenModel.Origin));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.RoutenModel.Waypoint));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.RoutenModel.Destination));
        }

        [TestMethod]
        public void DisplayMapReturnsErrorIfAllFieldsAreEmpty()
        {
            // Arrange
            viewModel.ClearFieldsCommand.Execute();

            // Act
            viewModel.DisplayMapCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.RoutenModel.Error));
        }

        [TestMethod]
        public void DisplayMapReturnsMapIfOnlyOneFieldIsSet()
        {
            // Arrange
            SetupGeoCodingMock(geoCodingMock);
            SetupStaticMapsMock(staticMapsMock);
            viewModel.ClearFieldsCommand.Execute();
            viewModel.RoutenModel.Origin = origin;
            
            // Act
            viewModel.DisplayMapCommand.Execute();

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.RoutenModel.Error));
        }

        [TestMethod]
        public void DisplayDirectionsReturnsErrorIfOriginFieldIsEmpty()
        {
            // Arrange
            viewModel.RoutenModel.Origin = string.Empty;

            // Act
            viewModel.DisplayDirectionsCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.RoutenModel.Error));
        }

        [TestMethod]
        public void DisplayDirectionsReturnsErrorIfDestinationFieldIsEmpty()
        {
            // Arrange
            viewModel.RoutenModel.Destination = string.Empty;

            // Act
            viewModel.DisplayDirectionsCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.RoutenModel.Error));
        }

        [TestMethod]
        public void DisplayDirectionsReturnsDirectionsIfOriginAndDestinationAreSet()
        {
            // Arrange
            SetupGeoCodingMock(geoCodingMock);
            SetupDirectionsMock(directionsMock);

            // Act
            viewModel.DisplayDirectionsCommand.Execute();

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.RoutenModel.Error));
            Assert.IsNull(viewModel.RoutenModel.MapView);
            Assert.IsTrue(viewModel.RoutenModel.Directions.Length > 0);
        }

        [TestMethod]
        public void DisplayDirectionsReturnsErrorIfOriginIsEmpty()
        {
            // Arrange
            SetupGeoCodingMock(geoCodingMock);
            SetupDirectionsMock(directionsMock);
            viewModel.RoutenModel.Origin = string.Empty;

            // Act
            viewModel.DisplayDirectionsCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.RoutenModel.Error));
        }

        [TestMethod]
        public void DisplayDirectionsReturnsErrorIfDestinationIsEmpty()
        {
            // Arrange
            SetupGeoCodingMock(geoCodingMock);
            SetupDirectionsMock(directionsMock);
            viewModel.RoutenModel.Destination = string.Empty;

            // Act
            viewModel.DisplayDirectionsCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.RoutenModel.Error));
        }


        [TestMethod]
        public void DisposeTest()
        {
            // Constructor of viewModel works as designed
            Assert.AreEqual(true, viewModel.DisplayMapCanExecute);

            //Change Origin property to empty
            viewModel.RoutenModel.Origin = string.Empty;
            Assert.AreEqual(false, viewModel.DisplayMapCanExecute);

            // Act
            viewModel.Dispose();

            // Eventhandler is disposed, so DisplayMapCanExecute can not be changed any more
            viewModel.RoutenModel.Origin = "Bern";
            Assert.AreEqual(false, viewModel.DisplayMapCanExecute);
        }
        #endregion TestMethods

        #region private methods
        private void SetupGeoCodingMock(Mock<IGeoCoding> geoCodingMock)
        {
            geoCodingMock.Reset();
            geoCodingMock.Setup(result => result.GetAdresses(origin)).Returns(Task.FromResult(GetLocations(0)));
            geoCodingMock.Setup(result => result.GetAdresses(via)).Returns(Task.FromResult(GetLocations(1)));
            geoCodingMock.Setup(result => result.GetAdresses(destination)).Returns(Task.FromResult(GetLocations(2)));
        }

        private void SetupStaticMapsMock(Mock<IStaticMaps> staticMapsMock)
        {
            staticMapsMock.Reset();
            staticMapsMock.Setup(result => result.GetMap(It.IsAny<IEnumerable<LocationDto>>())).Returns(Task.FromResult(GetMap()));
        }

        private void SetupDirectionsMock(Mock<IDirections> directionsMock)
        {
            directionsMock.Reset();
            directionsMock.Setup(result => result.GetDirections(It.IsAny<IEnumerable<LocationDto>>())).Returns(Task.FromResult(GetDirection()));
        }
        
        private IEnumerable<LocationDto> GetLocations(int counter)
        {
            var locations = new List<LocationDto>()
            {
                new LocationDto() { FormatedAddress = retOrigin, Longitude = 40, Latitude = 7, Error = string.Empty },
                new LocationDto() { FormatedAddress = retVia, Longitude = 41, Latitude = 8, Error = string.Empty },
                new LocationDto() { FormatedAddress = retDstination, Longitude = 42, Latitude = 9, Error = string.Empty}
            };

            yield return locations[counter];
        }

        private StreamDto GetMap()
        {
            var dto = new StreamDto() { Error = string.Empty, Stream = new MemoryStream()};
            using (FileStream file = new FileStream("..\\..\\test.gif", FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                dto.Stream.Write(bytes, 0, (int)file.Length);
            }
            return dto;
        }

        private DirectionDto GetDirection()
        {
            var Legs = new List<Leg>
            {
                {
                    new Leg()
                    {
                        Distance = new Distance() { Text = "10 km"},
                        Duration = new Duration() { Text = "15 min"},
                        Steps = new List<Step>()
                        {
                            new Step()
                            {
                                Distance = new Distance2() { Text = "2 km"},
                                Duration = new Duration2() { Text = "3 min"},
                                Html_instructions = "Goto somewhere"
                            },
                            new Step()
                            {
                                Distance = new Distance2() { Text = "8 km"},
                                Duration = new Duration2() { Text = "12 min"},
                                Html_instructions = "Goto somewhere elsde"
                            }
                        }
                    }
                }
            };

            return new DirectionDto()
            {
                Directions = "TestDirections", Routes = new RootObject()
                {
                    Routes = new List<Route>()
                    {
                        new Route()
                        {
                            Legs = Legs
                        }
                    },
                    Status="OK"
                }
            };
        }

        #endregion private methods
    }
}
