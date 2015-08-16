using System;
using System.ServiceModel;
using Hyper.NodeServices;
using Hyper.WcfHosting;

namespace NodeService1
{
    /* To setup a HyperNode service:
     * 
     * 1) Add the "Reactive Extensions - Main Library" NuGet package
     *        -When HyperNet DLLs are made into their own NuGet package, need to list reactive extensions as NuGet dependency
     * 2) Include the following HyperNet libraries:
     *        -Hyper.ActivityTracking.dll
     *        -Hyper.Extensibility.dll
     *        -Hyper.NodeServices.dll
     *        -Hyper.NodeServices.Client.dll
     *        -Hyper.NodeServices.Contracts.dll
     *        -Hyper.NodeServices.Extensibility.dll
     *        -Hyper.WcfHosting.dll
     */
    class Program
    {
        static void Main(string[] args)
        {
            HostingAttempt1();
        }

        public static void HostingAttempt1()
        {
            // This method has a couple of problems...
            try
            {
                /* In the first place, this line can throw exceptions if the HyperNode configuration section hasn't been setup in app.config
                 * The configuration is hidden inside the HyperNodeService.Instance property, but of primary importance are the cancellation
                 * settings. The standard ServiceHost object does not have all the functionality we might need for cancellation to work
                 * properly in our HyperNodes.
                 */
                var host = new ServiceHost(HyperNodeService.Instance);

                Console.WriteLine("Starting service...");

                /* Secondly, this line can throw various kinds of exceptions you may want to handle in different ways. In particular,
                 * it is recommended to handle the TimeoutException and the CommunicationException, all of which are handled the
                 * same way as any arbitrary exception.
                 */
                host.Open();

                Console.WriteLine("Service started and is listening on the following addresses:");
                foreach (var endpoint in host.Description.Endpoints)
                {
                    Console.WriteLine("    " + endpoint.Address);
                }

                // Wait for user input to shutdown
                Console.ReadKey();

                /* Finally, naively calling the Close() method can cause strange and unstable behavior. The recommended way
                 * to close a ServiceHost object is to check if its state is Faulted. If it is Faulted, then the Abort()
                 * method should be called instead of the Close() method.
                 * 
                 * TimeoutExceptions and CommunicationExceptions can be thrown on this method just like with the Open() method,
                 * and you could just as easily want to handle those exceptions in specific ways here as for the Open() method.
                 * 
                 * Finally, a little known fact of the ServiceHost object is that although calling Close() will dipsose of a disposable
                 * service in most cases, it does NOT automatically call Dispose() on singleton services. Because the HyperNodeService
                 * class *does* implement IDisposable, so the Dispose() method needs to be called on the HyperNode's
                 * singleton instance when you are finished with it.
                 */
                host.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadKey();

            /* Problem summary:
             * 
             * 1) Add the HyperNode configuration section to the app.config
             * 2) What do we do about the ServiceHost not being good enough?
             * 3) Check the host for the Faulted state and call Abort() instead of Close()
             * 4) Manually dispose of the HyperNodeService
             */
        }

        public static void HostingAttempt2()
        {
            /* Here we try to solve some of the above problems, but we haven't quite got there yet. */
            IDisposable disposableService = null;
            try
            {
                /* - We've added the HyperNode section to the app.config
                 * - What do we do about the cancellation settings? */
                var host = new ServiceHost(HyperNodeService.Instance);

                Console.WriteLine("Starting service...");
                host.Open();

                Console.WriteLine("Service started and is listening on the following addresses:");
                foreach (var endpoint in host.Description.Endpoints)
                {
                    Console.WriteLine("    " + endpoint.Address);
                }

                // Wait for user input to shutdown
                Console.ReadKey();
                
                // Save a reference to the singleton instance of the service and safe cast to IDisposable
                disposableService = host.SingletonInstance as IDisposable;

                // Check for Faulted state and make the appropriate call to Abort() or Close()
                if (host.State == CommunicationState.Faulted)
                    host.Abort();
                else
                    host.Close();
            }
            catch (TimeoutException exTimeout)
            {
                // Handle TimeoutException
            }
            catch (CommunicationException exCommunication)
            {
                // Handle CommunicationException
            }
            catch (Exception ex)
            {
                // Handle other generic exceptions
                Console.WriteLine(ex);
            }
            finally
            {
                // Dispose of our service if applicable
                if (disposableService != null)
                    disposableService.Dispose();
            }

            Console.ReadKey();

            /* Problem summary:
             * 
             * 1) Fixed most of the problems above
             * 2) What do we do about the ServiceHost not being good enough?
             */
        }

        public static void HostingAttempt3()
        {
            /* In order to solve the cancellation issues, we need to add functionality to the ServiceHost class to handle cancellation.
             * A little exploration into the HyperNodeService.Instance using intellisense tells us there is a Cancel() method and a
             * WaitAllChildTasks() method. Based on this information, we can make an educated guess that the service is multithreaded
             * and keeps track of a list of child tasks, each of which would need to be cancelled when the Close() or Abort() methods
             * are called on the ServiceHost. Presumably the Cancel() method issues the cancellation request, but now we have to figure
             * out how we can call the Cancel() method when either the Abort() or Close() methods are called. Easy, right?
             */
            IDisposable disposableService = null;
            try
            {
                var host = new ServiceHost(HyperNodeService.Instance);

                Console.WriteLine("Starting service...");
                host.Open();

                Console.WriteLine("Service started and is listening on the following addresses:");
                foreach (var endpoint in host.Description.Endpoints)
                {
                    Console.WriteLine("    " + endpoint.Address);
                }

                // Wait for user input to shutdown
                Console.ReadKey();

                // Save a reference to the singleton instance of the service and safe cast to IDisposable
                disposableService = host.SingletonInstance as IDisposable;

                // Cast the service as a HyperNodeService and call Cancel(). Then we'll be nice and wait for all child tasks to complete.
                HyperNodeService.Instance.Cancel();
                HyperNodeService.Instance.WaitAllChildTasks();

                // Check for Faulted state and make the appropriate call to Abort() or Close()
                if (host.State == CommunicationState.Faulted)
                    host.Abort();
                else
                    host.Close();
            }
            catch (TimeoutException exTimeout)
            {
                // Handle TimeoutException
            }
            catch (CommunicationException exCommunication)
            {
                // Handle CommunicationException
            }
            catch (Exception ex)
            {
                // Handle other generic exceptions
                Console.WriteLine(ex);
            }
            finally
            {
                // Dispose of our service if applicable
                if (disposableService != null)
                    disposableService.Dispose();
            }

            Console.ReadKey();

            /* Problem summary:
             * 
             * 1) We can now support cancellation
             * 2) Now there's the probably of portability;
             *    - All of the exception handlers are hard-coded and must be copy/pasted to be reused.
             *    - If the hosting program is intended to host more than one service, this code must be duplicated and slightly modified for each one
             *    - All of the cancellation clean-up is hard-coded in a linear fashion right before the abort.
             *    
             * 3) Enter the CancellableServiceHost. Benefits:
             *    - Injectable cancellation delegate registration
             * 4) Enter the HyperServiceHostContainer. Benefits:
             *    - Injectable exception handlers
             *    - Injectable service host factories
             *    - Wraps all the exception handling and closing details of ServiceHost objects and gives you a simple start/stop interface to use instead
             */
        }

        public static void HostingAttempt4()
        {
            /* Now using the CancellableServiceHost, we get this*/
            IDisposable disposableService = null;
            try
            {
                var host = new CancellableServiceHost(HyperNodeService.Instance);

                // Inject the cancellation tasks as a registered cancellation delegate. This can be done at any time, even after the host has opened
                host.RegisterCancellationDelegate(
                    () =>
                    {
                        HyperNodeService.Instance.Cancel();
                        HyperNodeService.Instance.WaitAllChildTasks();
                    }
                );

                Console.WriteLine("Starting service...");
                host.Open();

                Console.WriteLine("Service started and is listening on the following addresses:");
                foreach (var endpoint in host.Description.Endpoints)
                {
                    Console.WriteLine("    " + endpoint.Address);
                }

                // Wait for user input to shutdown
                Console.ReadKey();

                // Save a reference to the singleton instance of the service and safe cast to IDisposable
                disposableService = host.SingletonInstance as IDisposable;

                /* Check for Faulted state and make the appropriate call to Abort() or Close(). Both will ensure
                 * that the cancellation delegates are executed 
                 */
                if (host.State == CommunicationState.Faulted)
                    host.Abort();
                else
                    host.Close();
            }
            catch (TimeoutException exTimeout)
            {
                // Handle TimeoutException
            }
            catch (CommunicationException exCommunication)
            {
                // Handle CommunicationException
            }
            catch (Exception ex)
            {
                // Handle other generic exceptions
                Console.WriteLine(ex);
            }
            finally
            {
                // Dispose of our service if applicable
                if (disposableService != null)
                    disposableService.Dispose();
            }

            Console.ReadKey();

            /* Problem summary:
             * 
             * 1) Just one more thing to do: wrap in the HyperServiceHostContainer
             */
        }

        public static void HostingAttempt5()
        {
            /* Final product*/
            var container = new HyperServiceHostContainer(
                () =>
                {
                    var host = new CancellableServiceHost(HyperNodeService.Instance);

                    // Inject the cancellation tasks as a registered cancellation delegate. This can be done at any time, even after the host has opened
                    host.RegisterCancellationDelegate(
                        () =>
                        {
                            HyperNodeService.Instance.Cancel();
                            HyperNodeService.Instance.WaitAllChildTasks();
                        }
                    );

                    return host;
                },
                new DefaultServiceHostExceptionHandler() // Inject the exception hanlders we want to use
            );

            Console.WriteLine("Starting service...");

            // Start() returns a boolean indicating whether the service was able to be started. This allows graceful recovery from failure to start properly.
            if (!container.Start())
            {
                Console.WriteLine("Failed to start service. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Service started and is listening on the following addresses:");
            foreach (var endpoint in container.Endpoints)
            {
                Console.WriteLine("    " + endpoint.Address);
            }

            // Wait for user input to shutdown
            Console.ReadKey();

            // Simply call Stop() to handle all details of closing, including disposing the service, calling abort or close appropriately, and executing our cancellation delegates
            container.Stop();

            Console.ReadKey();
        }
    }
}
