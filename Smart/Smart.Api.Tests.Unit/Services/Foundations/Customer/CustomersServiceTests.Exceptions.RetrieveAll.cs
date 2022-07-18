using System;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Smart.Api.Models.Customer.Exceptions;
using Xunit;

namespace Smart.Api.Tests.Unit.Services.Foundations.Customer
{
    public partial class CustomersServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageException =
                new FailedCustomersStorageException(sqlException);

            var expectedCustomersDependencyException =
                new CustomersDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCustomer())
                    .Throws(sqlException);

            // when
            Action retrieveAllCustomerAction = () =>
                this.customersService.RetrieveAllCustomer();

            CustomersDependencyException actualCustomersDependencyException =
                Assert.Throws<CustomersDependencyException>(retrieveAllCustomerAction);

            // then
            actualCustomersDependencyException.Should()
                .BeEquivalentTo(expectedCustomersDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCustomer(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomersDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomMessage();
            var serviceException = new Exception(exceptionMessage);

            var failedCustomersServiceException =
                new FailedCustomersServiceException(serviceException);

            var expectedCustomersServiceException =
                new CustomersServiceException(failedCustomersServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCustomer())
                    .Throws(serviceException);

            // when
            Action retrieveAllCustomerAction = () =>
                this.customersService.RetrieveAllCustomer();

            CustomersServiceException actualCustomersServiceException =
                Assert.Throws<CustomersServiceException>(retrieveAllCustomerAction);

            // then
            actualCustomersServiceException.Should()
                .BeEquivalentTo(expectedCustomersServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCustomer(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomersServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}