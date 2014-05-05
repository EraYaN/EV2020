% EPO-4: script for fitting range-power radar equation
% MTSR, EWI, TU Delft, 2012
% contact: o.a.krasnov@tudelft.nl
%======================================================================

close all
clear all
clc
% Comment this line as soon as you inserted your group's measurement data
%h=msgbox('Do not forget to edit the script: enter your group''s measurement data','Attention!','warn','modal'); uiwait(h);
%======================================================================
% Enter your data here
%======================================================================
strGroup='EPO-4 group B-1'                    % put your group number here
Range = [3,3.5,4,4.5,5,5.5,6,6.5]; % put your set of ranges here
Power = [74.904, 59.965, 50.458, 42.414, 35.624, 30.191, 25.595, 21.312]  ;      % put your set of measured signal amplitudes
Power = 20*log10(Power*10^-3);
%======================================================================
disp('If Power = C/Range^A, then');
disp('log(Power) = -A * log(Range) + log(C)');
disp('to solve this and find A and log(C)');
disp('See "Linear Model with Nonpolynomial Terms" in MATLAB help');
disp('Note: Copy the  resulting figure into your report and try ');
disp('      to explain the deviation from theoretical expectation');

logPower=(Power)'./10;
logRange=log10(Range)';

X = [ones(size(logRange))  logRange];
a = X\logPower;

C = 10^(a(1));
A = -a(2);

Power1=(a(1)+logRange.*(-4));

figure
hold on
plot(logRange,(logPower).*10,'ro','linewidth',2)
plot(logRange,(-A*logRange +a(1)).*10,'b-','linewidth',2)
plot(logRange,(-4*logRange +a(1)+(-A+4)*logRange(3)).*10,'k-','linewidth',2)
set(gca,'Xlim',[0,1])
set(gca,'XTick',[0:0.1:1])
xlabel('log_{10}(Range)')
ylabel('Power, dB')
title({'Range dependence of radar signal in log-log scale',['and fitting results. ',strGroup]})
legend_handle=legend('Experiment',['Best fit: Power = C/Range^A = ','C','/Range^{',num2str(A),'}'],'Theory');
set(legend_handle,'Location','SouthWest')
hold off
grid on