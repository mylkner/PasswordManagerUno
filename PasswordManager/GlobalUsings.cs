global using System.Collections.Immutable;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using PasswordManager.Models;
global using PasswordManager.Presentation;
global using PasswordManager.Services;
global using PasswordManager.Services.Interfaces;
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;
global using Color = Windows.UI.Color;

[assembly: Uno.Extensions.Reactive.Config.BindableGenerationTool(3)]
