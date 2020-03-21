using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Diff.Strazzere.Anti.Debugger;
using Diff.Strazzere.Anti.Emulator;
using Diff.Strazzere.Anti.Monkey;
using Diff.Strazzere.Anti.Taint;

namespace AntiEmulatorTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

             
            var r1 = IsTaintTrackingDetected();

            var r2 = IsMonkeyDetected();

            var r3 = IsDebugged();

            var r4 = IsQEmuEnvDetected(); 

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool IsQEmuEnvDetected()
        {
            Log("Checking for QEmu env...");
            Log("hasKnownDeviceId : " + FindEmulator.HasKnownDeviceId(this));
            Log("hasKnownPhoneNumber : " + FindEmulator.HasKnownPhoneNumber(this));
            Log("isOperatorNameAndroid : " + FindEmulator.IsOperatorNameAndroid(this));
            Log("hasKnownImsi : " + FindEmulator.HasKnownImsi(this));
            Log("hasEmulatorBuild : " + FindEmulator.HasEmulatorBuild(this));
            Log("hasPipes : " + FindEmulator.HasPipes);
            Log("hasQEmuDriver : " + FindEmulator.HasQEmuDrivers);
            Log("hasQEmuFiles : " + FindEmulator.HasQEmuFiles);
            Log("hasGenyFiles : " + FindEmulator.HasGenyFiles);
            Log("hasEmulatorAdb :" + FindEmulator.HasEmulatorAdb);
            foreach (string abi in Build.SupportedAbis)
            {
                if (abi.Equals("armeabi-v7a", StringComparison.InvariantCultureIgnoreCase))
                {
                    Log("hitsQemuBreakpoint : " + FindEmulator.CheckQemuBreakpoint());
                }
            }
            if (FindEmulator.HasKnownDeviceId(this)
                            || FindEmulator.HasKnownImsi(this)
                            || FindEmulator.HasEmulatorBuild(this)
                            || FindEmulator.HasKnownPhoneNumber(this) || FindEmulator.HasPipes
                            || FindEmulator.HasQEmuDrivers || FindEmulator.HasEmulatorAdb
                            || FindEmulator.HasQEmuFiles
                            || FindEmulator.HasGenyFiles)
            {
                Log("QEmu environment detected.");
                return true;
            }
            else
            {
                Log("QEmu environment not detected.");
                return false;
            }
        }

        public bool IsTaintTrackingDetected()
        {
            Log("Checking for Taint tracking...");
            Log("hasAppAnalysisPackage : " + FindTaint.HasAppAnalysisPackage(this));
            Log("hasTaintClass : " + FindTaint.HasTaintClass);
            Log("hasTaintMemberVariables : " + FindTaint.HasTaintMemberVariables);
            if (FindTaint.HasAppAnalysisPackage(this) || FindTaint.HasTaintClass
                            || FindTaint.HasTaintMemberVariables)
            {
                Log("Taint tracking was detected.");
                return true;
            }
            else
            {
                Log("Taint tracking was not detected.");
                return false;
            }
        }

        public bool IsMonkeyDetected()
        {
            Log("Checking for Monkey user...");
            Log("isUserAMonkey : " + FindMonkey.IsUserAMonkey);

            if (FindMonkey.IsUserAMonkey)
            {
                Log("Monkey user was detected.");
                return true;
            }
            else
            {
                Log("Monkey user was not detected.");
                return false;
            }
        }

        public bool IsDebugged()
        {
            Log("Checking for debuggers...");

            bool tracer = false;
            try
            {
                tracer = FindDebugger.HasTracerPid;
            }
            catch (Exception exception)
            {
                Log(exception.Message);
            }

            if (FindDebugger.IsBeingDebugged || tracer)
            {
                Log("Debugger was detected");
                return true;
            }
            else
            {
                Log("No debugger was detected.");
                return false;
            }
        }

        public void Log(String msg)
        {
            Android.Util.Log.Verbose("AntiEmulator", msg);
        }
    }
}

