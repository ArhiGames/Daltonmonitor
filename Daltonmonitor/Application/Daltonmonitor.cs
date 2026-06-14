using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types.Common.Result;

namespace Daltonmonitor.Application;

public delegate void RunningStateChanged(bool newState);

public class Daltonmonitor
{
    private ConfigManager ConfigManager { get; } = new();
    private SubstitutionReader SubstitutionReader { get; }
    private HtmlGenerator HtmlGenerator { get; }

    private CancellationTokenSource? _cancellationTokenSource;
    private PeriodicTimer? _periodicTimer;

    public event RunningStateChanged? OnRunningStateChanged;

    public Daltonmonitor()
    {
        SubstitutionReader = new SubstitutionReader(ConfigManager);
        HtmlGenerator = new HtmlGenerator(ConfigManager);
    }

    public void ToggleRunningApplicationLogic()
    {
        if (_cancellationTokenSource is not null)
        {
            StopScanningLogic();
            return;
        }
        
        StopScanningLogic();
        Process();

        int scanIntervalSeconds = Convert.ToInt32(ConfigManager.GetConfigValue(ConfigIdentifier.CheckInterval));
        _cancellationTokenSource = new CancellationTokenSource();
        _periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(scanIntervalSeconds));

        OnRunningStateChanged?.Invoke(true);

        Task.Run(async () =>
        {
            try
            {
                while (await _periodicTimer.WaitForNextTickAsync(_cancellationTokenSource.Token))
                {
                    Process();
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                _periodicTimer.Dispose();
                _periodicTimer = null;
            }
        });
    }

    private void StopScanningLogic()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        
        OnRunningStateChanged?.Invoke(false);
    }

    private void Process()
    {
        ConfigManager.ReadConfigFile();
        
        Result<Timetable> timetableResult = SubstitutionReader.Process();
        if (!timetableResult.IsSuccess)
        {
            return;
        }
        
        Timetable timetable = timetableResult.Value!;
        string htmlStructure = HtmlGenerator.GenerateHtmlStructure(timetable);

        string outputFile = ConfigManager.GetConfigValue(ConfigIdentifier.OutputPath);
        try
        {
            File.WriteAllText(outputFile, htmlStructure);
        }
        catch
        {
            // ignored
        }

        SubstitutionReader.HandleUserMode();
    }
}