using Objects;
using Speckle.Automate.Sdk;
using Speckle.Core.Api.GraphQL.Models;

namespace SpecklePowerPivotForRevit;

public static class AutomateFunction
{
  public static async Task Run(
    AutomationContext automationContext,
    FunctionInputs functionInputs
  )
  {
    Console.WriteLine("Starting execution");
    _ = typeof(ObjectsKit).Assembly; // INFO: Force objects kit to initialize

    Console.WriteLine("Receiving version");
    var versionObject = await automationContext.ReceiveVersion();

    // Get the source model name
    var sourceModelName = await TriggerModelName(automationContext);

    // Generate the target model name
    var targetModelName = GenerateTargetModelName(
      sourceModelName,
      functionInputs.TargetModelPrefix
    );

    Console.WriteLine("Generated target model name: " + targetModelName);
    Console.WriteLine("Received version: " + versionObject);

    automationContext.MarkRunSuccess($"Targeting Model: {targetModelName}");
  }

  private static async Task<string> TriggerModelName(
    AutomationContext automationContext
  )
  {
    var modelId = automationContext.AutomationRunData.Triggers[0].Payload.ModelId;
    var versionId = automationContext.AutomationRunData.Triggers[0].Payload.VersionId;
    var projectId = automationContext.AutomationRunData.ProjectId;

    Console.WriteLine($"Project ID: {projectId}");
    Console.WriteLine($"Version ID: {versionId}");

    var client = automationContext.SpeckleClient;

    return (await client.Model.Get(modelId, projectId)).name;
  }

  private static string GenerateTargetModelName(string sourceModelName, string prefix)
  {
    // Ensure the prefix doesn't start with a slash
    prefix = prefix.TrimStart('/');

    // Split the source model name into parts
    var parts = sourceModelName.Split('/', StringSplitOptions.RemoveEmptyEntries);

    // Combine the prefix with the original model path
    var targetModelName = $"{prefix}/{string.Join("/", parts)}";

    return targetModelName;
  }
}