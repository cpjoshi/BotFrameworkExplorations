import './App.css';
import { Router, Route, Switch } from "react-router-dom";
import * as createHistory from "history";
import configuration from './components/configuration';
import Introduction from './components/introduction';
import SSOPage from './components/ssopage';
import { MessageForm } from './components/MessageForm';
import { kycform } from './components/kycform';
import AppLaunch from './components/AppLaunch';
import LaunchConfirmation from './components/LaunchConfirmation';

export const history = createHistory.createBrowserHistory();

function App() {
  return (
      <Router history={history}>
          <div>
              <Switch>
                  <Route path="/config" component={configuration} />
                  <Route path="/intro" component={Introduction} />
                  <Route path="/sso" component={SSOPage} />
                  <Route path="/message" component={MessageForm} />
                  <Route path="/kycform" component={kycform} />
                  <Route path="/apps" component={AppLaunch} />
                  <Route path="/launch" component={LaunchConfirmation} />
              </Switch>
          </div>
      </Router>  );
}

export default App;
