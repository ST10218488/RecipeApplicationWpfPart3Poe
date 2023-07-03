using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RecipeApplicationWpfPart3Poe
{
    public partial class MainWindow : Window
    {
        private List<Recipe> recipes;
        private Recipe selectedRecipe;

        public MainWindow()
        {
            InitializeComponent();
            recipes = new List<Recipe>();
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            string recipeName = txtRecipeName.Text.Trim();
            if (!string.IsNullOrEmpty(recipeName))
            {
                Recipe recipe = new Recipe(recipeName);
                recipes.Add(recipe);
                lstRecipes.ItemsSource = GetFilteredRecipes();
                ClearRecipeInputs();
            }
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe.");
                return;
            }

            string ingredientName = txtIngredientName.Text.Trim();
            int calories;
            if (!string.IsNullOrEmpty(ingredientName) && int.TryParse(txtCalories.Text, out calories))
            {
                string unitOfMeasurement = txtUnitOfMeasurement.Text.Trim();
                string quantity = txtQuantity.Text.Trim();
                string foodGroup = (cmbFoodGroup.SelectedItem as ComboBoxItem)?.Content.ToString();
                Ingredient ingredient = new Ingredient(ingredientName, calories, unitOfMeasurement, quantity, foodGroup);
                selectedRecipe.AddIngredient(ingredient);
                lstIngredients.ItemsSource = selectedRecipe.Ingredients;
                txtTotalCalories.Text = selectedRecipe.CalculateTotalCalories().ToString();

                if (selectedRecipe.CalculateTotalCalories() > 300)
                {
                    MessageBox.Show("Total calories exceed 300!");
                }

                ClearIngredientInputs();
            }
        }

    
        private void lstRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRecipe = lstRecipes.SelectedItem as Recipe;
            if (selectedRecipe != null)
            {
                txtSelectedRecipe.Text = selectedRecipe.Name;
                lstIngredients.ItemsSource = selectedRecipe.Ingredients.Select(i => new { IngredientInfo = $"{i.Name} - {i.Quantity}{i.UnitOfMeasurement} - {i.FoodGroup}" }).ToList();
                txtTotalCalories.Text = selectedRecipe.CalculateTotalCalories().ToString();
            }
        }


        private void txtFilterIngredient_TextChanged(object sender, TextChangedEventArgs e)
        {
            lstRecipes.ItemsSource = GetFilteredRecipes();
        }

        private void cmbFilterFoodGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstRecipes.ItemsSource = GetFilteredRecipes();
        }

        private void txtFilterMaxCalories_TextChanged(object sender, TextChangedEventArgs e)
        {
            lstRecipes.ItemsSource = GetFilteredRecipes();
        }

       private List<Recipe> GetFilteredRecipes()
        {
            string filterIngredient = txtFilterIngredient.Text.Trim().ToLower();
            string filterFoodGroup = (cmbFilterFoodGroup.SelectedItem as ComboBoxItem)?.Content.ToString();
            int filterMaxCalories;
          int.TryParse(txtFilterMaxCalories.Text, out filterMaxCalories);

            var filteredRecipes = recipes.Where(recipe =>
                (string.IsNullOrEmpty(filterIngredient) || recipe.Ingredients.Any(ingredient => ingredient.Name.ToLower().Contains(filterIngredient))) &&
                (string.IsNullOrEmpty(filterFoodGroup) || recipe.Ingredients.Any(ingredient => ingredient.FoodGroup == filterFoodGroup)) &&
                (filterMaxCalories == 0 || recipe.CalculateTotalCalories() <= filterMaxCalories)
            ).OrderBy(r => r.Name).ToList();

            return filteredRecipes;
        }
      
        private void ClearRecipeInputs()
        {
            txtRecipeName.Text = string.Empty;
        }

        private void ClearIngredientInputs()
        {
            txtIngredientName.Text = string.Empty;
            txtCalories.Text = string.Empty;
            txtUnitOfMeasurement.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            cmbFoodGroup.SelectedItem = null;
        }
    }

    public class Recipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; } // Update to a public get property

        public Recipe(string name)
        {
            Name = name;
            Ingredients = new List<Ingredient>();
        }

        public void AddIngredient(Ingredient ingredient)
        {
            Ingredients.Add(ingredient);
        }

        public int CalculateTotalCalories()
        {
            return Ingredients.Sum(i => i.Calories);
        }
     

    }


    public class Ingredient
    {
        public string Name { get; set; }
        public int Calories { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string Quantity { get; set; }
        public string FoodGroup { get; set; }

        public Ingredient(string name, int calories, string unitOfMeasurement, string quantity, string foodGroup)
        {
            Name = name;
            Calories = calories;
            UnitOfMeasurement = unitOfMeasurement;
            Quantity = quantity;
            FoodGroup = foodGroup;
        }

        public string IngredientInfo => $"{Name} - {Quantity} {UnitOfMeasurement} - {FoodGroup}";
    }

}
