using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owin;
using Simple.Web.Http;

namespace Simple.Web.Owin
{
	public static class SelfHostingApp
	{
		public static void App(IDictionary<string, object> env, ResultDelegate result, Action<Exception> fault)
		{

			try {
				var wrapper = new ContextWrapper(env);

				var appTask = new Application()
					.Run(wrapper)
					.ToApm(CallCompleted(wrapper, result), null);

				appTask.Wait();
			} catch (Exception ex){
				fault(ex);
			}
		}

		static AsyncCallback CallCompleted(IContext context, ResultDelegate result) {
			return ar => {
				var response = (ResponseWrapper)context.Response;
				var task = ar as Task<object>;
				if (task != null && task.IsFaulted)
				{
					OwinOutput.SendFailureResult(context, result, task);
				}
				else
				{
					OwinOutput.SendSuccessResult(result, response);
				}
			};
		}

		public static void Use<T>() where T:new()
		{
			new T();
		}
	}
}