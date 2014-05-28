y = x_10_90;

x = y(:,1);
max = 0;
max_val = 0;
for i = 1:length(x)
    if abs(x(i)) > max_val
        max = i;
        max_val = abs(x(i));
    end
end

max_1 = max;

x = y(:,2);
max = 0;
max_val = 0;
for i = 1:length(x)
    if abs(x(i)) > max_val
        max = i;
        max_val = abs(x(i));
    end
end

max_2 = max;

disp('Distance in cm:')
dist = 343.2 * abs(max_1 - max_2) / 48000 * 100