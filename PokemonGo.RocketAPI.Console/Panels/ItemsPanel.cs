/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 14/09/2016
 * Time: 22:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;
using System.Threading.Tasks;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Helpers;
	
namespace PokemonGo.RocketAPI.Console
{
	/// <summary>
	/// Description of ItemsPanel.
	/// </summary>
	public partial class ItemsPanel : UserControl
	{
		public ItemsPanel()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void BtnRealoadItemsClick(object sender, EventArgs e)
		{
			ItemsListView.Items.Clear();
            Execute();
		}
		public async void Execute()
        {
            try
            {
                num_MaxPokeballs.Value = Globals.pokeball;
                num_MaxGreatBalls.Value = Globals.greatball;
                num_MaxUltraBalls.Value = Globals.ultraball;
                num_MaxRevives.Value = Globals.revive;
                num_MaxPotions.Value = Globals.potion;
                num_MaxSuperPotions.Value = Globals.superpotion;
                num_MaxHyperPotions.Value = Globals.hyperpotion;
                num_MaxRazzBerrys.Value = Globals.berry;
                num_MaxTopRevives.Value = Globals.toprevive;
                num_MaxTopPotions.Value = Globals.toppotion;
                int count = 0;
                count += Globals.pokeball + Globals.greatball + Globals.ultraball + Globals.revive
                    + Globals.potion + Globals.superpotion + Globals.hyperpotion + Globals.berry
                    + Globals.toprevive + Globals.toppotion;
                text_TotalItemCount.Text = count.ToString();

                var client = Logic.Logic._client;
	            if (client.readyToUse != false)
	            {
	               var items = await client.Inventory.GetItems();
	              
	               ItemId[] validsIDs = {ItemId.ItemPokeBall,ItemId.ItemGreatBall,ItemId.ItemUltraBall};
	               
	               ListViewItem listViewItem;
	               ItemsListView.Items.Clear();
	               var sum = 0;
	               foreach (  var item in items) {
	                listViewItem = new ListViewItem();
	                listViewItem.Tag = item;
	                listViewItem.Text = getItemName(item.ItemId);
	                listViewItem.ImageKey = item.ItemId.ToString().Replace("Item","");
	                listViewItem.SubItems.Add(""+item.Count);
	                sum +=item.Count;
	                listViewItem.SubItems.Add(""+item.Unseen);
	                ItemsListView.Items.Add(listViewItem);
	               }
	               lblCount.Text=""+ sum;
	            }
            }
            catch (Exception e)
            {

                Logger.Error("[ItemsList-Error] " + e.StackTrace);
                await Task.Delay(1000); // Lets the API make a little pause, so we dont get blocked
                Execute();
            }
        }
		private string getItemName(ItemId itemID)
        {
            switch (itemID)
            {
                case ItemId.ItemPotion:
                    return "Potion";
                case ItemId.ItemSuperPotion:
                    return "Super Potion";
                case ItemId.ItemHyperPotion:
                    return "Hyper Potion";
                case ItemId.ItemMaxPotion:
                    return "Max Potion";
                case ItemId.ItemRevive:
                    return "Revive";
                case ItemId.ItemIncenseOrdinary:
                    return "Incense";
                case ItemId.ItemPokeBall:
                    return "Poke Ball";
                case ItemId.ItemGreatBall:
                    return "Great Ball";
                case ItemId.ItemUltraBall:
                    return "Ultra Ball";
                case ItemId.ItemRazzBerry:
                    return "Razz Berry";
                case ItemId.ItemIncubatorBasic:
                    return "Egg Incubator";
                case ItemId.ItemIncubatorBasicUnlimited:
                    return "Unlimited Egg Incubator";
                default:
                    return itemID.ToString().Replace("Item", "");
            }
        }
		async void RecycleToolStripMenuItemClick(object sender, EventArgs e)
        {

            var item = (ItemData)ItemsListView.SelectedItems[0].Tag;
            int amount = IntegerInput.ShowDialog(1, "How many?", item.Count);
            if (amount > 0)
            {
                taskResponse resp = new taskResponse(false, string.Empty);

                resp = await RecycleItems(item, amount);
                if (resp.Status)
                {
                    item.Count -= amount;
                    ItemsListView.SelectedItems[0].SubItems[1].Text = "" + item.Count;
                }
                else
                    MessageBox.Show(resp.Message + " recycle failed!", "Recycle Status", MessageBoxButtons.OK);

            }
        }
		public class taskResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public taskResponse() { }
            public taskResponse(bool status, string message)
            {
                Status = status;
                Message = message;
            }
        }
		private static async Task<taskResponse> RecycleItems(ItemData item, int amount)
        {
            taskResponse resp1 = new taskResponse(false, string.Empty);
            try
            {
            	var client = Logic.Logic._client;
                var resp2 = await client.Inventory.RecycleItem(item.ItemId, amount);

                if (resp2.Result == RecycleInventoryItemResponse.Types.Result.Success)
                {
                    resp1.Status = true;
                }
                else
                {
                    resp1.Message = item.ItemId.ToString();
                }
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error RecycleItem: " + e.Message);
                await RecycleItems(item, amount);
            }
            return resp1;
        }		
        private void num_Max(object sender, EventArgs e)
        {
        	try{
        		var numB = (NumericUpDown) sender;
        		var value = (int) numB.Value;
                //Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "==========Begin Recycle Filter Debug Logging=============");
                //Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "Value Setter Triggered for: " + numB.Name + " New Value: " + numB.Value);
                //Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "==========End Recycle Filter Debug Logging=============");
                switch (numB.Name) {
        			case "num_MaxPokeballs":
        				Globals.pokeball = value;
        			break;
        			case "num_MaxGreatBalls":
        				Globals.greatball = value;
        			break;
        			case "num_MaxUltraBalls":
        				Globals.ultraball = value;
        			break;
        			case "num_MaxRevives":
        				Globals.revive = value;
        			break;
        			case "num_MaxPotions":
        				Globals.potion = value;
        			break;
        			case "num_MaxSuperPotions":
        				Globals.superpotion = value;
        			break;
        			case "num_MaxHyperPotions":
        				Globals.hyperpotion = value;
        			break;
        			case "num_MaxTopRevives":
        				Globals.toprevive = value;
        			break;
        			case "num_MaxTopPotions":
        				Globals.toppotion = value;
        			break;
        			case "num_MaxRazzBerrys":        		
        				Globals.berry = value;
        			break;
        				
        		}        
        		 int count = 0;
		            count += Globals.pokeball + Globals.greatball + Globals.ultraball + Globals.revive
		                + Globals.potion + Globals.superpotion + Globals.hyperpotion + Globals.berry 
		                + Globals.toprevive + Globals.toppotion;
		         text_TotalItemCount.Text = count.ToString();
        	}catch (Exception e1){
        		
        	}
        }
        void btnCopy_Click(object sender, EventArgs e)
        {
          foreach ( ListViewItem lvitem in ItemsListView.Items) {
                var item = (ItemData) (lvitem.Tag);
                var controlName = item.ItemId.ToString();
                controlName = controlName.Replace("Item","num_Max") + "s";
                var controls = Controls.Find(controlName,true);
                if (controls.Length > 0 )
                {
                    ((NumericUpDown)controls[0]).Value = item.Count;
                }
          }
        }
        
        private async Task RecycleItems(bool forcerefresh = false)
        {            
            var client = Logic.Logic._client;
            var items = await client.Inventory.GetItemsToRecycle(new Settings());
            foreach (var item in items)
            {
                var transfer = await client.Inventory.RecycleItem((ItemId)item.ItemId, item.Count);
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Recycled {item.Count}x {(ItemId)item.ItemId}", LogLevel.Info);
                await RandomHelper.RandomDelay(1000, 5000);
            }
        }

        async void btnDiscard_Click(object sender, EventArgs e)
        {
            await RecycleItems();
            Execute();
        }

        void useToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = (ItemData)ItemsListView.SelectedItems[0].Tag;
            if ((item.ItemId != ItemId.ItemLuckyEgg) && (item.ItemId != ItemId.ItemIncenseOrdinary))
            {
                MessageBox.Show(getItemName( item.ItemId) + " cannot be used here.");
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Are you sure you want use " + getItemName( item.ItemId) +"?", "Use Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (item.ItemId == ItemId.ItemIncenseOrdinary)
                {
                    Globals.UseIncenseGUIClick = true;
                }
                if (item.ItemId == ItemId.ItemLuckyEgg)
                {
                    Globals.UseLuckyEggGUIClick = true;
                }
            
            }

        }		
	}
}
