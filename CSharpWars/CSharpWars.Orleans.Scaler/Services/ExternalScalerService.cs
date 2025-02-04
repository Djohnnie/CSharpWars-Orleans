using Externalscaler;
using Grpc.Core;

namespace CSharpWars.Orleans.Scaler.Services;

public class ExternalScalerService : ExternalScaler.ExternalScalerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly IManagementGrain _managementGrain;
    private readonly ILogger<ExternalScalerService> _logger;
    private readonly string _metricName = "grainThreshold";

    public ExternalScalerService(
        IClusterClient clusterClient,
        ILogger<ExternalScalerService> logger)
    {
        _clusterClient = clusterClient;
        _logger = logger;
        _logger.LogInformation("ExternalScalerService created.");
        _managementGrain = clusterClient.GetGrain<IManagementGrain>(0);
        _logger.LogInformation("ExternalScalerService IManagementGrain received.");

    }

    public override async Task<GetMetricsResponse> GetMetrics(GetMetricsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("GetMetrics called.");

        CheckRequestMetadata(request.ScaledObjectRef);

        var response = new GetMetricsResponse();
        var grainType = request.ScaledObjectRef.ScalerMetadata["graintype"];
        var siloNameFilter = request.ScaledObjectRef.ScalerMetadata["siloNameFilter"];
        var upperbound = Convert.ToInt32(request.ScaledObjectRef.ScalerMetadata["upperbound"]);
        var fnd = await GetGrainCountInCluster(grainType, siloNameFilter);
        long grainsPerSilo = (fnd.GrainCount > 0 && fnd.SiloCount > 0) ? (fnd.GrainCount / fnd.SiloCount) : 0;
        long metricValue = fnd.SiloCount;

        if (grainsPerSilo < upperbound)
        {
            metricValue = fnd.GrainCount == 0 ? 1 : Convert.ToInt16(fnd.GrainCount / upperbound);
        }

        if (grainsPerSilo >= upperbound)
        {
            metricValue = fnd.SiloCount + 1;
        }

        if (metricValue == 0)
        {
            metricValue = 1;
        }

        _logger.LogInformation($"Grains Per Silo: {grainsPerSilo}, Upper Bound: {upperbound}, Grain Count: {fnd.GrainCount}, Silo Count: {fnd.SiloCount}. Scale to {metricValue}.");

        response.MetricValues.Add(new MetricValue
        {
            MetricName = _metricName,
            MetricValue_ = metricValue
        });

        return response;
    }

    public override Task<GetMetricSpecResponse> GetMetricSpec(ScaledObjectRef request, ServerCallContext context)
    {
        _logger.LogInformation("GetMetricSpec called.");

        CheckRequestMetadata(request);

        var resp = new GetMetricSpecResponse();

        resp.MetricSpecs.Add(new MetricSpec
        {
            MetricName = _metricName,
            TargetSize = 1
        });

        return Task.FromResult(resp);
    }

    public override async Task<IsActiveResponse> IsActive(ScaledObjectRef request, ServerCallContext context)
    {
        _logger.LogInformation("IsActive called.");

        CheckRequestMetadata(request);

        var result = await AreTooManyGrainsInTheCluster(request);
        _logger.LogInformation($"Returning {result} from IsActive.");
        return new IsActiveResponse
        {
            Result = result
        };
    }

    public override async Task StreamIsActive(ScaledObjectRef request, IServerStreamWriter<IsActiveResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("StreamIsActive called.");

        CheckRequestMetadata(request);

        while (!context.CancellationToken.IsCancellationRequested)
        {
            if (await AreTooManyGrainsInTheCluster(request))
            {
                _logger.LogInformation($"Writing IsActiveResopnse to stream with Result = true.");
                await responseStream.WriteAsync(new IsActiveResponse
                {
                    Result = true
                });
            }

            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }

    private static void CheckRequestMetadata(ScaledObjectRef request)
    {
        if (!request.ScalerMetadata.ContainsKey("graintype")
            || !request.ScalerMetadata.ContainsKey("upperbound")
             || !request.ScalerMetadata.ContainsKey("siloNameFilter"))
        {
            throw new ArgumentException("graintype, siloNameFilter, and upperbound must be specified");
        }
    }

    private async Task<bool> AreTooManyGrainsInTheCluster(ScaledObjectRef request)
    {
        var grainType = request.ScalerMetadata["graintype"];
        var upperbound = request.ScalerMetadata["upperbound"];
        var siloNameFilter = request.ScalerMetadata["siloNameFilter"];
        var counts = await GetGrainCountInCluster(grainType, siloNameFilter);
        if (counts.GrainCount == 0 || counts.SiloCount == 0) return false;
        var tooMany = Convert.ToInt32(upperbound) <= (counts.GrainCount / counts.SiloCount);
        return tooMany;
    }

    private async Task<GrainSaturationSummary> GetGrainCountInCluster(string grainType, string siloNameFilter)
    {
        var statistics = await _managementGrain.GetDetailedGrainStatistics();
        var activeGrainsInCluster = statistics.Select(x => new GrainInfo(x.GrainType, x.GrainId.ToString(), x.SiloAddress.ToGatewayUri().AbsoluteUri));
        var grainsInClusterCount = activeGrainsInCluster.Count();
        _logger.LogInformation($"Found a total of {grainsInClusterCount} grains in cluster.");
        var activeGrainsOfSpecifiedType = activeGrainsInCluster.Where(x => x.Type.ToLower().Contains(grainType));
        var detailedHosts = await _managementGrain.GetDetailedHosts();
        var hostCount = detailedHosts.Count(x => x.Status == SiloStatus.Active);
        _logger.LogInformation($"Found a total of {hostCount} silos in cluster.");
        var silos = detailedHosts
                        .Where(x => x.Status == SiloStatus.Active)
                        .Select(x => new SiloInfo(x.HostName, x.SiloAddress.ToGatewayUri().AbsoluteUri));
        var activeSiloCount = silos.Where(x => x.SiloName.ToLower().Contains(siloNameFilter.ToLower())).Count();
        _logger.LogInformation($"Found {activeGrainsOfSpecifiedType.Count()} instances of {grainType} in cluster, with {activeSiloCount} '{siloNameFilter}' silos in the cluster hosting {grainType} grains.");
        return new GrainSaturationSummary(activeGrainsOfSpecifiedType.Count(), activeSiloCount);
    }
}

public record GrainInfo(string Type, string PrimaryKey, string SiloName);
public record GrainSaturationSummary(long GrainCount, long SiloCount);
public record SiloInfo(string SiloName, string SiloAddress);