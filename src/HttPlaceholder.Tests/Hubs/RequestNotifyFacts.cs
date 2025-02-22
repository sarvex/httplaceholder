﻿using System.Linq;
using AutoMapper;
using HttPlaceholder.Hubs.Implementations;
using HttPlaceholder.Web.Shared.Dto.v1.Requests;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Tests.Hubs;

[TestClass]
public class RequestNotifyFacts
{
    private readonly AutoMocker _mocker = new();
    private Mock<IClientProxy> _clientProxyMock;

    [TestInitialize]
    public void Initialize()
    {
        var mocks = TestObjectFactory.CreateHubMock<RequestHub>();
        _mocker.Use(mocks.hubContext);
        _clientProxyMock = mocks.clientProxyMock;
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task NewRequestReceivedAsync_HappyFlow()
    {
        // Arrange
        var mapperMock = _mocker.GetMock<IMapper>();
        var notify = _mocker.CreateInstance<RequestNotify>();

        var input = new RequestResultModel();
        var mappedDto = new RequestOverviewDto();
        mapperMock
            .Setup(m => m.Map<RequestOverviewDto>(input))
            .Returns(mappedDto);

        // Act
        await notify.NewRequestReceivedAsync(input, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("RequestReceived", It.Is<object[]>(o => o.Single() == mappedDto),
            It.IsAny<CancellationToken>()));
    }
}
