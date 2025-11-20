using EvaComponentLibrary.Models;
using EvaComponentLibrary.Services;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using System.Net.Security;



namespace EvaComponentLibrary.ViewModels
{
	public class CustomInstViewModel
	{
		public CustomInstruction CunstInstsList = new CustomInstruction();
		public event Action? UpdateUi;
		public event Action<bool>? ServerResponse;
		public event Action<bool>? RemoveResult;
		public event Action<bool>? AddResult;
		public event Action<bool>? EditResult;
		public event Action<bool>? SetIndexResult;

		public readonly WebServices _ws;

		public CustomInstViewModel(WebServices service)
		{
			_ws = service ?? throw new ArgumentNullException(nameof(service));
		}



		public async Task<bool> AddNewOrEditInst( string? headline, string inst, int? index)
		{
			if (!string.IsNullOrEmpty(inst) && index != null && index > -1 && index < CunstInstsList.CunstInst.Count)
			{
				CunstInstsList.CunstInst[(int)index].Inst = inst;
				CunstInstsList.CunstInst[(int)index].Title = headline; 
				bool isSucc = await PutInstAsync();
				EditResult?.Invoke(isSucc);
				return true;
			}
			else if (!string.IsNullOrEmpty(inst))
			{
				var newInstruction = new Instruction
				{
					Inst = inst,
					Title = headline
				};
				CunstInstsList.CunstInst.Add(newInstruction); 
				int i = CunstInstsList.CunstInst.IndexOf(newInstruction);
				await SetInstIndex(i);
				bool isSucc = await PutInstAsync();
				AddResult?.Invoke(isSucc);
				return true;
			}


			return false;

		}
		public  async Task<bool> AddNewOrEditRes( string? headline, string res, int? index)
		{
			if (!string.IsNullOrEmpty(res) && index != null && index > -1 && index < CunstInstsList.CunstRes.Count)
			{
				CunstInstsList.CunstRes[(int)index].Resp = res;
				CunstInstsList.CunstRes[(int)index].Title = headline;
                bool isSucc = await PutInstAsync();
				EditResult?.Invoke(isSucc);
				return true;
			}
			else if (!string.IsNullOrEmpty(res))
			{

				var newResponse = new Response
				{
                                   
                    Resp = res,
                    Title = headline
                
                };

                CunstInstsList.CunstRes.Add(newResponse);
				int i = CunstInstsList.CunstRes.IndexOf(newResponse);
				await SetResIndex(i);
				bool isSucc = await PutInstAsync();
				AddResult?.Invoke(isSucc);
				return true;

			}

			return false;
		}


		public async Task<bool> RemoveCunstInst(int index)
		{
			if (index > -1 && index < CunstInstsList.CunstInst.Count)
			{
				CunstInstsList.CunstInst.RemoveAt(index);
				bool isSucc = await PutInstAsync();
				RemoveResult?.Invoke(isSucc);
				return true;

			}
			return false;
		}


		public async Task<bool> RemoveCunstRes(int index)
		{
			if (index > -1 && index < CunstInstsList.CunstRes.Count)
			{
				CunstInstsList.CunstRes.RemoveAt(index);
				bool isSucc = await PutInstAsync();
				RemoveResult?.Invoke(isSucc);
				return true;

			}
			return false;
		}


		public async Task<bool> SetInstIndex(int index)
		{
			try
			{
				CunstInstsList.InstIndex = index;
				bool isSucc = await PutInstAsync();
				SetIndexResult?.Invoke(isSucc);
				return true;

			}
			catch (Exception ex)
			{
				//(ex.ToString());
				return false;
			}


		}
		public async Task<bool> SetResIndex(int index)
		{

			try
			{
				CunstInstsList.ResIndex = index;
				bool isSucc = await PutInstAsync();
				SetIndexResult?.Invoke(isSucc);
				return true;
			}
			catch (Exception ex)
			{
				//(ex.ToString());
				return false;

			}

		}

		public async Task GetInstAsync()
		{
			string defaultInstruction = string.Empty;

			try
			{
				defaultInstruction = await _ws.GetInstAsync();
				//(defaultInstruction);
				CunstInstsList = JsonSerializer.Deserialize<CustomInstruction>(defaultInstruction);
				UpdateUi?.Invoke();

			}
			catch (Exception e)
			{
				//($"{e.ToString()}");

			}
		}


		public async Task<bool> PutInstAsync()
		{

			bool isSucc = await _ws.PutInstAsync(CunstInstsList);
			if (isSucc) 
			{
				UpdateUi?.Invoke();
				return true;
			}
			return false;

		}
	}
}
