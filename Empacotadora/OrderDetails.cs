﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Empacotadora {
	public class OrderDetails {
		// Properties
		public string ID { get; set; }
		public string Active;
		public string Name { get; set; }
		public string Diameter { get; set; }
		public string Width { get; set; }
		public string Height { get; set; }
		public string Thick { get; set; }
		public string Length { get; set; }
		public string Density { get; set; }
		public string TubeAm { get; set; }
		public string TubeType { get; set; }
		public string PackageType { get; set; }
		public string Weight { get; set; }
		public string Created { get; set; }
		public byte Straps { get; set; }
		public int[] StrapsPosition { get; set; }
		// Methods
		public double CalculateWeight(OrderDetails order) {
			double weight;
			bool boolDiam = double.TryParse(order.Diameter, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double diameter);
			bool boolWidth = double.TryParse(order.Width, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double width);
			bool boolHeight = double.TryParse(order.Height, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double height);
			double.TryParse(order.Thick, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double thickness);
			double.TryParse(order.Length, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double length);
			double.TryParse(order.Density, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double density);
			if (boolDiam && !(boolWidth && boolHeight)) {
				double diameterOut = diameter;
				double diameterIn = diameter - thickness;
				weight = ((Math.PI * ((Math.Pow((0.5 * diameterOut), 2)) -
											(Math.Pow((0.5 * diameterIn), 2)))) * length * (density * 0.000001));
			}
			else {
				weight = (((height * width * length) - (((height - (2 * thickness)) *
							(width - (2 * thickness))) * length)) * (density * 1000) * 0.000000001);
			}
			return weight;
		}
		public static ICollection<OrderDetails> ReadOrdersFromFile(string path) {
			ICollection<OrderDetails> orders = new Collection<OrderDetails>();
			if (!Document.ReadFromFile(path, out IEnumerable<string> linesFromFile)) return orders;
			foreach (string line in linesFromFile) {
				string[] array = line.Split(',');
				try {
					if (array[1] == "1") {
						orders.Add(new OrderDetails() {
							ID = array[0],
							Name = array[2],
							Diameter = array[3],
							Width = array[4],
							Height = array[5],
							Thick = array[6],
							Length = array[7],
							Density = array[8],
							TubeAm = array[9],
							TubeType = array[10],
							PackageType = array[11],
							Weight = array[12],
							Created = array[13],
						});
					}
				}
				catch (IndexOutOfRangeException exc) {
					MessageBox.Show(exc.Message);
					return orders;
				}
			}
			return orders;
		}
		public static bool DeactivateOrder(string path, string orderID) {
			if (!Document.ReadFromFile(path, out IEnumerable<string> linesFromFile)) return false;
			ICollection<string> newFileContent = new Collection<string>();
			foreach (string line in linesFromFile) {
				string newline = "";
				string[] array = line.Split(',');
				if (array[0] == orderID) {
					array[1] = "0";
					//foreach (string value in array)
					//	newline += value + ",";
					newline = array.Aggregate(newline, (current, value) => current + (value + ","));
					// removes "," in the end of the line
					newline = newline.Remove(newline.Length - 1);
				}
				newFileContent.Add(newline == "" ? line : newline);
			}
			return Document.WriteToFile(path, newFileContent.ToArray());
		}
		public static bool EditOrder(string path, string orderID, string[] valuesToWrite) {
			if (!Document.ReadFromFile(path, out IEnumerable<string> linesFromFile)) return false;
			ICollection<string> newFileContent = new Collection<string>();
			foreach (string line in linesFromFile) {
				string[] array = line.Split(',');
				string newLine = "";
				if (array[0] == orderID) {
					for (int i = 0; i <= 11; i++)
						newLine += valuesToWrite[i] + ",";
					newLine += array[12] + ",";
					newLine += array[13];
				}
				newFileContent.Add(newLine == "" ? line : newLine);
			}
			return Document.WriteToFile(path, newFileContent.ToArray());
		}
	}
}
