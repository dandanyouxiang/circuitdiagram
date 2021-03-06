﻿// winNewComponentImplementation.xaml.cs
//
// Circuit Diagram http://www.circuit-diagram.org/
//
// Copyright (C) 2012-2014  Sam Fisher
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using CircuitDiagram.Components;
using CircuitDiagram.Components.Description;
using CircuitDiagram.DPIWindow;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CircuitDiagram
{
    /// <summary>
    /// Interaction logic for winNewComponentImplementation.xaml
    /// </summary>
    public partial class winNewComponentImplementation : MetroDPIWindow
    {
        public winNewComponentImplementation()
        {
            InitializeComponent();

            btnOK.IsEnabled = false;

            // Populate representation combobox
            UpdateChoices();

            this.DPIChanged += winNewComponentImplementation_DPIChanged;
        }

        void winNewComponentImplementation_DPIChanged(object sender, EventArgs e)
        {
            var imageConverter = this.Resources["MultiResolutionImageToIMageSourceConverter"] as MultiResolutionImageToImageSourceConverter;
            imageConverter.DPI = CurrentDPI;
        }

        private void UpdateChoices()
        {
            cbxRepresentation.Items.Clear();

            if (tbxImplementItem.Text.Length == 0)
            {
                cbxRepresentation.IsEnabled = false;
                return;
            }

            cbxRepresentation.IsEnabled = true;

            foreach (ComponentDescription description in ComponentHelper.ComponentDescriptions.Where(c => c.Metadata.ImplementSet == this.ImplementationSet))
            {
                if (description.Metadata.Configurations.Count == 0 && description.Metadata.ImplementItem == tbxImplementItem.Text)
                {
                    ImplementationConversion item = new ImplementationConversion();
                    item.ToName = description.ComponentName;
                    item.ToGUID = description.Metadata.GUID;
                    if (description.Metadata.Icon != null)
                        item.ToIcon = description.Metadata.Icon;
                    cbxRepresentation.Items.Add(item);
                }
                else
                {
                    foreach (ComponentConfiguration configuration in description.Metadata.Configurations.Where(c => c.ImplementationName == tbxImplementItem.Text))
                    {
                        ImplementationConversion item = new ImplementationConversion();
                        item.ToName = description.ComponentName;
                        item.ToGUID = description.Metadata.GUID;
                        if (configuration.Icon == null && description.Metadata.Icon != null)
                            item.ToIcon = description.Metadata.Icon;
                        else
                            item.ToIcon = configuration.Icon;
                        item.ToConfiguration = configuration.Name;
                        cbxRepresentation.Items.Add(item);
                    }
                }
            }

            cbxRepresentation.SelectedIndex = 0;
        }

        public string ImplementationSet
        {
            get { return tbxImplementSet.Text; }
            set { tbxImplementSet.Text = value; }
        }

        public ImplementationConversion GetChosenComponent()
        {
            ImplementationConversion item = cbxRepresentation.SelectedItem as ImplementationConversion;
            item.ImplementationName = tbxImplementItem.Text;
            return item;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void tbxImplementItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateChoices();
            btnOK.IsEnabled = (tbxImplementItem.Text.Length > 0 && cbxRepresentation.Items.Count > 0);
        }
    }
}
