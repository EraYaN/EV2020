incl = 0;
simout0 = zeros(length(speeds),1);
for i = 1:length(speeds),
idx=i;
sim(model);
simout0(i) = P_batt(150000);
end;
incl = 3;
simout3 = zeros(length(speeds),1);
for i = 1:length(speeds),
idx=i;
sim(model);
simout3(i) = P_batt(150000);
end;
figure();
plot(speeds,[simout0,simout3]);
legend('Power at 0% incl.','Power at 3% incl.');