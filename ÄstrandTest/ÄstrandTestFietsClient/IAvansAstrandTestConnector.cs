using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestFietsClient
{
    public interface IAvansAstrandTestConnector
    {
        void OnAstrandTestStart();
        void OnAstrandTestEnd(bool hasSteadyState, double vo2);
        void OnAstrandTestAbort(string message);
        void OnAstrandTestToFast();
        void OnAstrandTestToSlow();
        void OnAstrandTestGoodSpeed();
        void OnAstrandTestSetResistance(int resistance);
        void OnAstrandTestSteadyStateReached();
        void OnAstrandTestLastHeartrateMeasured(int heartrate);
        void OnAstrandTestLogStateAndCountdown(AvansAstrandTest.State state, string timestring);
    }
}
