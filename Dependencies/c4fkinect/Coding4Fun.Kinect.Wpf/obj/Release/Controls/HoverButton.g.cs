﻿#pragma checksum "..\..\..\Controls\HoverButton.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "84AA50087CD1A4CEC216A314C279D132"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Expression.Shapes;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Coding4Fun.Kinect.Wpf.Controls {
    
    
    /// <summary>
    /// HoverButton
    /// </summary>
    public partial class HoverButton : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\Controls\HoverButton.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Coding4Fun.Kinect.Wpf.Controls.HoverButton UserControl;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\Controls\HoverButton.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualStateGroup VisualStateGroup;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Controls\HoverButton.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState Normal;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\Controls\HoverButton.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState Checked;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\Controls\HoverButton.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid uxContentHolder;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\Controls\HoverButton.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image uxButtonImage;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\Controls\HoverButton.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image uxButtonActiveImage;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\Controls\HoverButton.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Expression.Shapes.Arc uxRing;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Coding4Fun.Kinect.Wpf;component/controls/hoverbutton.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\HoverButton.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.UserControl = ((Coding4Fun.Kinect.Wpf.Controls.HoverButton)(target));
            return;
            case 2:
            this.VisualStateGroup = ((System.Windows.VisualStateGroup)(target));
            return;
            case 3:
            this.Normal = ((System.Windows.VisualState)(target));
            return;
            case 4:
            this.Checked = ((System.Windows.VisualState)(target));
            return;
            case 5:
            this.uxContentHolder = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.uxButtonImage = ((System.Windows.Controls.Image)(target));
            return;
            case 7:
            this.uxButtonActiveImage = ((System.Windows.Controls.Image)(target));
            return;
            case 8:
            this.uxRing = ((Microsoft.Expression.Shapes.Arc)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

