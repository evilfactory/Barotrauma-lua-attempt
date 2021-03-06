﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Barotrauma.Networking;
using MoonSharp.Interpreter;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Barotrauma
{
	partial class LuaSetup
	{

		public Script lua;
		public LuaHook hook;
		public LuaGame game;

		public void HandleLuaException(Exception ex)
		{
			if(ex is InterpreterException)
			{
				PrintMessage(((InterpreterException)ex).DecoratedMessage);
			}
			else
			{
				PrintMessage(ex.ToString());
			}
		}

		public void PrintMessage(object message)
		{
			Console.WriteLine(message.ToString());
			if (GameMain.Server != null)
			{
				foreach (var c in GameMain.Server.ConnectedClients)
				{
					GameMain.Server.SendDirectChatMessage(message.ToString(), c, ChatMessageType.Console);
					GameServer.Log("[LUA] " + message.ToString(), ServerLog.MessageType.ServerMessage);
				}
			}
		}

		public void DoString(string code)
		{
			try
			{
				lua.DoString(code);
			}
			catch (Exception e)
			{
				HandleLuaException(e);
			}
		}


		public void RunFunction(DynValue func)
		{
			try
			{
				lua.Call(func);
			}
			catch (Exception e)
			{
				HandleLuaException(e);
			}
		}

		public void DoFile(string file)
		{
			try
			{
				lua.DoFile(file);
			}
			catch (Exception e)
			{
				HandleLuaException(e);
			}
		}


		public LuaSetup()
		{
			PrintMessage("Lua!");

			LuaScriptLoader luaScriptLoader = new LuaScriptLoader(this);

			LuaCustomConverters.RegisterAll();

			UserData.RegisterType<TraitorMessageType>();
			UserData.RegisterType<JobPrefab>();
			UserData.RegisterType<CharacterInfo>();
			UserData.RegisterType<Rectangle>();
			UserData.RegisterType<Point>();
			UserData.RegisterType<Level.InterestingPosition>();
			UserData.RegisterType<Level.PositionType>();
			UserData.RegisterType<Level>();
			UserData.RegisterType<Items.Components.Steering>();
			UserData.RegisterType<ServerLog.MessageType>();
			UserData.RegisterType<SpawnType>();
			UserData.RegisterType<ChatMessageType>();
			UserData.RegisterType<WayPoint>();
			UserData.RegisterType<Character>();
			UserData.RegisterType<Item>();
			UserData.RegisterType<Submarine>();
			UserData.RegisterType<Client>();
			UserData.RegisterType<LuaPlayer>();
			UserData.RegisterType<LuaHook>();
			UserData.RegisterType<LuaGame>();
			UserData.RegisterType<LuaRandom>();
			UserData.RegisterType<LuaTimer>();
			UserData.RegisterType<Vector2>();
			UserData.RegisterType<Vector3>();
			UserData.RegisterType<Vector4>();
			

			lua = new Script(CoreModules.Preset_SoftSandbox | CoreModules.LoadMethods);

			lua.Options.DebugPrint = PrintMessage;

			lua.Options.ScriptLoader = luaScriptLoader;

			hook = new LuaHook(this);
			game = new LuaGame(this);

			lua.Globals["Player"] = new LuaPlayer();
			lua.Globals["Game"] = game;
			lua.Globals["Hook"] = hook;
			lua.Globals["Random"] = new LuaRandom();
			lua.Globals["Timer"] = new LuaTimer(this);
			lua.Globals["WayPoint"] = UserData.CreateStatic<WayPoint>();
			lua.Globals["SpawnType"] = UserData.CreateStatic<SpawnType>();
			lua.Globals["ChatMessageType"] = UserData.CreateStatic<ChatMessageType>();
			lua.Globals["ServerLog_MessageType"] = UserData.CreateStatic<ServerLog.MessageType>();
			lua.Globals["Submarine"] = UserData.CreateStatic<Submarine>();
			lua.Globals["Client"] = UserData.CreateStatic<Client>();
			lua.Globals["Character"] = UserData.CreateStatic<Character>();
			lua.Globals["Item"] = UserData.CreateStatic<Item>();
			lua.Globals["Level"] = UserData.CreateStatic<Level>();
			lua.Globals["Vector2"] = UserData.CreateStatic<Vector2>();
			lua.Globals["Vector3"] = UserData.CreateStatic<Vector3>();
			lua.Globals["PositionType"] = UserData.CreateStatic<Level.PositionType>();
			lua.Globals["JobPrefab"] = UserData.CreateStatic<JobPrefab>();
			lua.Globals["TraitorMessageType"] = UserData.CreateStatic<TraitorMessageType>();

			foreach (string d in Directory.GetDirectories("Lua"))
			{
				if (Directory.Exists(d + "/autorun"))
				{
					luaScriptLoader.RunFolder(d + "/autorun");
				}
			}



		}

	}



}

